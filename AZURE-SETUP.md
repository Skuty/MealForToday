# Azure Deployment Setup Guide

This guide explains how to set up Azure authentication for automated deployments using GitHub Actions.

## Prerequisites

- Azure account (free tier available at https://azure.microsoft.com/free/)
- Azure CLI installed locally (for initial setup)
- Repository admin access to configure GitHub secrets

## Azure Authentication Setup

The workflow uses **Azure Federated Identity** (workload identity federation) for secure, password-less authentication from GitHub Actions.

### Step 1: Create Azure AD Application

1. **Login to Azure:**
   ```bash
   az login
   ```

2. **Create an Azure AD Application:**
   ```bash
   az ad app create --display-name "MealForToday-GitHub-Actions"
   ```
   
   Note the `appId` from the output - this is your `AZURE_CLIENT_ID`.

3. **Create a Service Principal:**
   ```bash
   APP_ID="<your-app-id-from-previous-step>"
   az ad sp create --id $APP_ID
   ```
   
   Note the `id` from the output - this is your Service Principal Object ID.

### Step 2: Configure Federated Credentials

Set up federated identity credentials to allow GitHub Actions to authenticate. 

**Important**: The manual deployment workflow uses GitHub environments (`test`, `staging`), so you **must** create the environment-specific federated credentials (shown below) in addition to the branch-based ones.

```bash
APP_ID="<your-app-id>"
GITHUB_ORG="Skuty"
GITHUB_REPO="MealForToday"

# For main/production environment
az ad app federated-credential create \
  --id $APP_ID \
  --parameters '{
    "name": "MealForToday-GitHub-Actions-Main",
    "issuer": "https://token.actions.githubusercontent.com",
    "subject": "repo:'$GITHUB_ORG'/'$GITHUB_REPO':ref:refs/heads/main",
    "audiences": ["api://AzureADTokenExchange"]
  }'

# For pull requests (test/staging environments)
az ad app federated-credential create \
  --id $APP_ID \
  --parameters '{
    "name": "MealForToday-GitHub-Actions-PR",
    "issuer": "https://token.actions.githubusercontent.com",
    "subject": "repo:'$GITHUB_ORG'/'$GITHUB_REPO':pull_request",
    "audiences": ["api://AzureADTokenExchange"]
  }'

# For any branch (allows manual deployments from feature branches)
az ad app federated-credential create \
  --id $APP_ID \
  --parameters '{
    "name": "MealForToday-GitHub-Actions-Branch",
    "issuer": "https://token.actions.githubusercontent.com",
    "subject": "repo:'$GITHUB_ORG'/'$GITHUB_REPO':ref:refs/heads/*",
    "audiences": ["api://AzureADTokenExchange"]
  }'

# For test environment (required for manual deployments using environment: test)
az ad app federated-credential create \
  --id $APP_ID \
  --parameters '{
    "name": "MealForToday-GitHub-Actions-Test-Env",
    "issuer": "https://token.actions.githubusercontent.com",
    "subject": "repo:'$GITHUB_ORG'/'$GITHUB_REPO':environment:test",
    "audiences": ["api://AzureADTokenExchange"]
  }'

# For staging environment (required for manual deployments using environment: staging)
az ad app federated-credential create \
  --id $APP_ID \
  --parameters '{
    "name": "MealForToday-GitHub-Actions-Staging-Env",
    "issuer": "https://token.actions.githubusercontent.com",
    "subject": "repo:'$GITHUB_ORG'/'$GITHUB_REPO':environment:staging",
    "audiences": ["api://AzureADTokenExchange"]
  }'
```

### Step 3: Assign Azure Permissions

Grant the service principal permissions to create and manage resources:

```bash
SUBSCRIPTION_ID=$(az account show --query id --output tsv)
SP_ID="<service-principal-object-id-from-step-1>"

# Grant Contributor role at subscription level
az role assignment create \
  --assignee $SP_ID \
  --role Contributor \
  --scope /subscriptions/$SUBSCRIPTION_ID

# Note: For production, you should use a more restrictive scope like a specific resource group
```

### Step 4: Configure GitHub Secrets

Add the following secrets to your GitHub repository:

1. Go to: **Repository Settings** → **Secrets and variables** → **Actions** → **New repository secret**

2. Add these secrets:

   | Secret Name | Value | How to Get |
   |-------------|-------|------------|
   | `AZURE_CLIENT_ID` | Your Application (client) ID | From Step 1, the `appId` output |
   | `AZURE_TENANT_ID` | Your Azure tenant ID | Run: `az account show --query tenantId -o tsv` |
   | `AZURE_SUBSCRIPTION_ID` | Your Azure subscription ID | Run: `az account show --query id -o tsv` |

### Step 5: Configure GitHub Environments (Optional)

For additional security and approval workflows:

1. Go to: **Repository Settings** → **Environments**
2. Create environments: `test` and `staging`
3. Configure protection rules (optional):
   - Required reviewers
   - Wait timer
   - Deployment branches

## Using the Automated Deployment

### Deploy to Azure Container Apps

1. Go to: **Actions** → **Manual Deploy to Test Environment**
2. Click **Run workflow**
3. Configure:
   - **Action**: `deploy`
   - **Environment**: `test` or `staging`
   - **Deploy to Azure**: ✅ (checked)
   - **Branch**: Select your branch
4. Click **Run workflow**

The workflow will:
- Build and push Docker image to GHCR
- Deploy to Azure Container Apps with auto-scaling (0-1 replicas)
- Provide the application URL

### Cleanup Azure Resources

1. Go to: **Actions** → **Manual Deploy to Test Environment**
2. Click **Run workflow**
3. Configure:
   - **Action**: `cleanup`
   - **Environment**: `test` or `staging`
4. Click **Run workflow**

The workflow will:
- Delete ALL Container Apps in the specified environment
- Keep the resource group and environment for future deployments

## Auto-Scaling Configuration

The deployment is configured with auto-scaling to 0:

```yaml
--min-replicas 0      # Scales to zero when idle (no traffic)
--max-replicas 1      # Scales up to 1 instance when traffic arrives
--cpu 0.25            # 0.25 vCPU per instance
--memory 0.5Gi        # 0.5 GiB memory per instance
```

**Benefits:**
- ✅ **Cost savings**: Only consumes resources when actively used
- ✅ **Free tier friendly**: Maximizes your 180k vCPU-seconds/month quota
- ✅ **Automatic**: No manual intervention needed
- ✅ **Fast startup**: Container Apps has optimized cold start (~2-5 seconds)

**Trade-offs:**
- ⚠️ **Cold start delay**: First request after idle takes 2-5 seconds
- ⚠️ **Data loss**: In-memory database resets when scaled to zero

## Resource Naming Convention

All resources are created in a single resource group, with environment and branch differentiation in the app names:

- **Resource Group**: `MealForToday` (shared across all environments)
- **Container App Environment**: `mealfortoday-{environment}-env`
- **Container App**: `mealfortoday-{environment}-{branch-name}`

Example for branch `feature/new-ui` in `test` environment:
- Resource Group: `MealForToday`
- Container App Environment: `mealfortoday-test-env`
- Container App: `mealfortoday-test-feature-new-ui`

## Cost Estimates

### Free Tier (First Month)
- **Azure Account**: $200 credit for 30 days
- **Azure Container Apps**: 180k vCPU-seconds + 360k GiB-seconds per month free

### Ongoing (After Free Trial)
With auto-scaling to 0, costs depend on actual usage:

- **Container App**: ~$0.000012 per vCPU-second, ~$0.000002 per GiB-second
- **Example**: 1 hour active testing per day
  - vCPU: 3600 seconds × 0.25 vCPU × $0.000012 = $0.0108/day
  - Memory: 3600 seconds × 0.5 GiB × $0.000002 = $0.0036/day
  - **Total**: ~$0.43/month per deployment
- **Idle time**: $0 (scaled to zero)

**Free tier covers**: ~3+ hours of active testing per day within quota.

## Troubleshooting

### Authentication Issues

**Problem**: "AADSTS70021: No matching federated identity record found" or "AADSTS700213: No matching federated identity record found for presented assertion subject 'repo:Skuty/MealForToday:environment:test'"

**Solution**: 
- Verify federated credentials are created for the correct repository
- Check the subject pattern matches your workflow trigger
- Ensure GitHub organization and repo names are correct
- **For environment-based deployments**: Make sure you've created federated credentials for each environment (test, staging) as shown in Step 2. The workflow uses `environment: test` which requires a matching federated credential with subject `repo:ORG/REPO:environment:test`

### Deployment Failures

**Problem**: "Resource group not found" or "Container app environment not found"

**Solution**:
- The workflow creates these automatically on first deployment
- Ensure service principal has Contributor role
- Check Azure subscription is active

### Permission Errors

**Problem**: "Authorization failed" or "Insufficient permissions"

**Solution**:
```bash
# Verify role assignment
az role assignment list --assignee $SP_ID --output table

# If missing, add Contributor role
az role assignment create \
  --assignee $SP_ID \
  --role Contributor \
  --scope /subscriptions/$SUBSCRIPTION_ID
```

### Cleanup Not Working

**Problem**: Cleanup workflow completes but resources still exist

**Solution**:
- Check Azure Portal for resources
- Note: The cleanup workflow deletes individual container apps, not the resource group
- To delete everything including the resource group: `az group delete --name MealForToday --yes`
- Verify branch name matches (hyphens instead of special characters)

## Security Best Practices

1. **Use Federated Identity**: Already configured - no passwords/secrets needed
2. **Restrict Permissions**: Consider using resource group scope instead of subscription
3. **Environment Protection**: Configure required reviewers for production
4. **Regular Cleanup**: Use cleanup workflow to remove unused deployments
5. **Monitor Costs**: Set up Azure cost alerts

## Alternative: Using Service Principal with Password (Not Recommended)

If you cannot use federated identity, you can use a service principal with password:

```bash
# Create service principal with password
az ad sp create-for-rbac \
  --name "MealForToday-GitHub-Actions" \
  --role Contributor \
  --scopes /subscriptions/$SUBSCRIPTION_ID \
  --sdk-auth

# Add the JSON output as a GitHub secret named AZURE_CREDENTIALS
# Update workflow to use:
# - uses: azure/login@v2
#   with:
#     creds: ${{ secrets.AZURE_CREDENTIALS }}
```

**Warning**: This method is less secure as it uses long-lived credentials.

## Next Steps

1. ✅ Complete Azure authentication setup (Steps 1-4)
2. ✅ Test deployment to Azure from a feature branch
3. ✅ Verify auto-scaling works (check Azure Portal metrics)
4. ✅ Test cleanup workflow
5. ✅ Set up GitHub environments for approval workflows (optional)

## Support

- Azure Container Apps docs: https://learn.microsoft.com/azure/container-apps/
- GitHub Actions Azure login: https://github.com/Azure/login
- Workload identity federation: https://learn.microsoft.com/azure/active-directory/workload-identities/workload-identity-federation
