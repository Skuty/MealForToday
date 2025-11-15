# Deployment Guide

This document describes how to deploy MealForToday application for testing purposes using the manual deployment workflow.

## Overview

The manual deployment workflow builds a Docker image from your PR branch and pushes it to GitHub Container Registry (GHCR). You can deploy this image to Azure Container Apps automatically or to various other environments manually.

## Prerequisites

- GitHub account with access to this repository
- (Optional) Azure account for automated cloud deployment - see [AZURE-SETUP.md](AZURE-SETUP.md)
- (Optional) Docker installed locally for self-hosted deployment

## Quick Start: Automated Azure Deployment

For the fastest deployment experience with auto-scaling and automatic cleanup:

1. **One-time setup**: Configure Azure authentication following [AZURE-SETUP.md](AZURE-SETUP.md)
2. Navigate to **Actions** → **"Manual Deploy to Test Environment"**
3. Click **"Run workflow"**
4. Configure:
   - **Action**: `deploy`
   - **Environment**: `test` or `staging`
   - **Deploy to Azure**: ✅ (checked)
   - **Branch**: Select your branch
5. Click **"Run workflow"**

The workflow will:
- Build and push Docker image to GHCR
- Deploy to Azure Container Apps with auto-scaling (0-1 replicas)
- Provide the application URL
- Post deployment info as PR comment (if triggered from PR)

**To cleanup**: Run workflow with **Action**: `cleanup`

See [Option 2: Automated Azure Container Apps Deployment](#option-2-automated-azure-container-apps-deployment) below for details.

## Manual Deployment Workflow

### Triggering a Deployment

1. Navigate to the **Actions** tab in the GitHub repository
2. Select **"Manual Deploy to Test Environment"** workflow
3. Click **"Run workflow"** button
4. Select:
   - **Action**: `deploy` (to deploy) or `cleanup` (to remove deployment)
   - **Branch**: Choose your PR branch or any branch you want to deploy
   - **Environment**: Select `test` or `staging`
   - **Deploy to Azure**: Check this to deploy to Azure Container Apps automatically
   - **PR Number** (optional): The PR number if deploying from a PR branch
5. Click **"Run workflow"**

The workflow will:
- Build a Docker image from your branch (if action is `deploy`)
- Push it to GitHub Container Registry (GHCR) at `ghcr.io/skuty/mealfortoday`
- Deploy to Azure Container Apps if selected
- Generate deployment instructions in the workflow summary

### Accessing the Docker Image

After the workflow completes, your image will be available at:
```
ghcr.io/skuty/mealfortoday:<branch-name>
```

## Deployment Options

### Option 1: Local/Self-Hosted Docker Deployment

This is the simplest option for quick testing on any machine with Docker.

#### Prerequisites
- Docker installed on your machine
- Access to pull images from GHCR (may require authentication for private repos)

#### Steps

1. **Authenticate with GHCR** (required for private repositories):
   ```bash
   echo $GITHUB_TOKEN | docker login ghcr.io -u USERNAME --password-stdin
   ```
   Replace `$GITHUB_TOKEN` with a personal access token with `read:packages` permission.

2. **Pull the image**:
   ```bash
   docker pull ghcr.io/skuty/mealfortoday:<branch-name>
   ```

3. **Run the container**:
   ```bash
   docker run -d \
     --name mealfortoday-test \
     -p 8080:8080 \
     -e ASPNETCORE_ENVIRONMENT=Development \
     ghcr.io/skuty/mealfortoday:<branch-name>
   ```

4. **Access the application**:
   - Open browser at `http://localhost:8080`
   - Use demo credentials from README.md

5. **View logs**:
   ```bash
   docker logs -f mealfortoday-test
   ```

6. **Stop and remove**:
   ```bash
   docker stop mealfortoday-test
   docker rm mealfortoday-test
   ```

#### Using Docker Compose

Alternatively, you can use the provided `docker-compose.yml`:

```bash
# Pull latest image
docker pull ghcr.io/skuty/mealfortoday:<branch-name>

# Update docker-compose.yml to use the image
# Replace the build section with:
# image: ghcr.io/skuty/mealfortoday:<branch-name>

# Start the application
docker-compose up -d

# View logs
docker-compose logs -f

# Stop the application
docker-compose down
```

### Option 2: Automated Azure Container Apps Deployment

**Recommended for cloud testing with auto-scaling to zero.**

Azure Container Apps deployment is now automated through the GitHub Actions workflow with built-in auto-scaling and cleanup.

#### Features
- ✅ **Automated deployment**: One-click deployment from GitHub Actions
- ✅ **Auto-scaling to 0**: Scales to zero when idle (saves costs)
- ✅ **Auto-scaling up**: Automatically handles traffic when it arrives
- ✅ **Automatic cleanup**: Remove deployments with cleanup action
- ✅ **Free tier friendly**: Optimized for 180k vCPU-seconds/month quota

#### One-Time Setup

Follow the [AZURE-SETUP.md](AZURE-SETUP.md) guide to:
1. Create Azure AD Application
2. Configure federated identity credentials
3. Assign Azure permissions
4. Add GitHub secrets (`AZURE_CLIENT_ID`, `AZURE_TENANT_ID`, `AZURE_SUBSCRIPTION_ID`)

#### Deploy to Azure

1. Go to **Actions** → **"Manual Deploy to Test Environment"**
2. Click **"Run workflow"**
3. Configure:
   - **Action**: `deploy`
   - **Environment**: `test` or `staging`
   - **Deploy to Azure**: ✅ (checked)
   - **Branch**: Select your branch
4. Click **"Run workflow"**

The workflow will:
- Build and push Docker image to GHCR
- Create Azure resources (resource group, container app environment) if needed
- Deploy container app with configuration:
  - **Auto-scaling**: 0-1 replicas (scales to zero when idle)
  - **Resources**: 0.25 vCPU, 0.5Gi memory
  - **Ingress**: External HTTPS endpoint
  - **Image**: Latest from your branch
- Provide the application URL (e.g., `https://mealfortoday-test-mybranch.region.azurecontainerapps.io`)

#### Cleanup Azure Deployment

When done testing, remove all deployments:

1. Go to **Actions** → **"Manual Deploy to Test Environment"**
2. Click **"Run workflow"**
3. Configure:
   - **Action**: `cleanup`
   - **Environment**: Same as deployment (e.g., `test`)
4. Click **"Run workflow"**

The workflow will:
- Delete ALL container apps in the specified environment
- Keep the resource group and environment for future deployments

#### Resource Naming

Resources are automatically named:
- **Resource Group**: `mealfortoday-{environment}-rg`
- **Container App Environment**: `mealfortoday-{environment}-env`
- **Container App**: `mealfortoday-{environment}-{branch-name}`

Example for branch `feature/ui-updates` in `test` environment:
- Container App: `mealfortoday-test-feature-ui-updates`

#### Auto-Scaling Details

The deployment uses Azure Container Apps' built-in auto-scaling:

```yaml
Min replicas: 0    # Scales to zero when no traffic
Max replicas: 1    # Scales to 1 when traffic arrives
CPU: 0.25 vCPU     # Per instance
Memory: 0.5Gi      # Per instance
```

**Behavior:**
- **No traffic**: Automatically scales to 0 replicas (no cost)
- **Traffic arrives**: Scales to 1 replica in ~2-5 seconds
- **Traffic stops**: Scales back to 0 after idle period (~1-2 minutes)

**Cost impact:**
- Only charged for actual usage time
- ~3+ hours of testing per day within free tier
- $0 when idle (scaled to zero)

### Option 3: Manual Azure Container Apps (Advanced)

If you prefer manual control or want to customize the deployment, you can use Azure CLI directly.

Azure Container Apps provides a free tier suitable for testing and development.

#### Free Tier Limits
- 180,000 vCPU-seconds per month
- 360,000 GiB-seconds per month
- Suitable for testing and development

#### Prerequisites
- Azure account (free tier available)
- Azure CLI installed
- Logged in: `az login`

#### Steps

1. **Set variables**:
   ```bash
   RESOURCE_GROUP="mealfortoday-test-rg"
   LOCATION="eastus"
   CONTAINER_APP_ENV="mealfortoday-env"
   CONTAINER_APP_NAME="mealfortoday-test"
   BRANCH_NAME="your-branch-name"
   ```

2. **Create resource group** (one-time):
   ```bash
   az group create \
     --name $RESOURCE_GROUP \
     --location $LOCATION
   ```

3. **Create Container Apps environment** (one-time):
   ```bash
   az containerapp env create \
     --name $CONTAINER_APP_ENV \
     --resource-group $RESOURCE_GROUP \
     --location $LOCATION
   ```

4. **Deploy the container**:
   ```bash
   az containerapp create \
     --name $CONTAINER_APP_NAME \
     --resource-group $RESOURCE_GROUP \
     --environment $CONTAINER_APP_ENV \
     --image ghcr.io/skuty/mealfortoday:$BRANCH_NAME \
     --target-port 8080 \
     --ingress external \
     --min-replicas 0 \
     --max-replicas 1 \
     --cpu 0.25 \
     --memory 0.5Gi \
     --env-vars ASPNETCORE_ENVIRONMENT=Production
   ```

5. **Get the application URL**:
   ```bash
   az containerapp show \
     --name $CONTAINER_APP_NAME \
     --resource-group $RESOURCE_GROUP \
     --query properties.configuration.ingress.fqdn \
     --output tsv
   ```

6. **Update the deployment** (when new image is available):
   ```bash
   az containerapp update \
     --name $CONTAINER_APP_NAME \
     --resource-group $RESOURCE_GROUP \
     --image ghcr.io/skuty/mealfortoday:$BRANCH_NAME
   ```

7. **Delete resources** (when done testing):
   ```bash
   az containerapp delete \
     --name $CONTAINER_APP_NAME \
     --resource-group $RESOURCE_GROUP \
     --yes
   ```

### Option 3: Azure Web App for Containers

Azure Web Apps also offers a free tier (F1) for testing.

#### Free Tier Limits (F1)
- 1 GB RAM
- 1 GB storage
- 60 CPU minutes/day
- Shared infrastructure

#### Prerequisites
- Azure account
- Azure CLI installed

#### Steps

1. **Set variables**:
   ```bash
   RESOURCE_GROUP="mealfortoday-test-rg"
   LOCATION="eastus"
   APP_SERVICE_PLAN="mealfortoday-plan"
   WEB_APP_NAME="mealfortoday-test-$(date +%s)"  # Unique name
   BRANCH_NAME="your-branch-name"
   ```

2. **Create resource group** (one-time):
   ```bash
   az group create \
     --name $RESOURCE_GROUP \
     --location $LOCATION
   ```

3. **Create App Service Plan** (one-time):
   ```bash
   az appservice plan create \
     --name $APP_SERVICE_PLAN \
     --resource-group $RESOURCE_GROUP \
     --is-linux \
     --sku F1
   ```

4. **Create and deploy Web App**:
   ```bash
   az webapp create \
     --name $WEB_APP_NAME \
     --resource-group $RESOURCE_GROUP \
     --plan $APP_SERVICE_PLAN \
     --deployment-container-image-name ghcr.io/skuty/mealfortoday:$BRANCH_NAME
   ```

5. **Configure container settings**:
   ```bash
   az webapp config appsettings set \
     --name $WEB_APP_NAME \
     --resource-group $RESOURCE_GROUP \
     --settings WEBSITES_PORT=8080 ASPNETCORE_ENVIRONMENT=Production
   ```

6. **Get the application URL**:
   ```bash
   echo "https://$WEB_APP_NAME.azurewebsites.net"
   ```

7. **Update the container** (when new image is available):
   ```bash
   az webapp config container set \
     --name $WEB_APP_NAME \
     --resource-group $RESOURCE_GROUP \
     --docker-custom-image-name ghcr.io/skuty/mealfortoday:$BRANCH_NAME
   
   az webapp restart \
     --name $WEB_APP_NAME \
     --resource-group $RESOURCE_GROUP
   ```

8. **Delete resources** (when done testing):
   ```bash
   az webapp delete \
     --name $WEB_APP_NAME \
     --resource-group $RESOURCE_GROUP
   ```

## Infrastructure Setup Summary

### What You Need (One-Time Setup)

#### For GitHub Container Registry (GHCR)
- ✅ **Already configured** in the repository
- No additional setup required
- Images are automatically pushed by the workflow
- Public repositories: Free unlimited storage
- Private repositories: Free for 500MB, then paid tiers

#### For Azure Cloud Deployment (Optional)

1. **Azure Account**:
   - Sign up at https://azure.microsoft.com/free/
   - Free tier includes $200 credit for 30 days
   - Free services available beyond trial period

2. **Azure CLI Installation**:
   ```bash
   # macOS
   brew install azure-cli
   
   # Windows
   # Download from https://aka.ms/installazurecliwindows
   
   # Linux
   curl -sL https://aka.ms/InstallAzureCLIDeb | sudo bash
   ```

3. **Login to Azure**:
   ```bash
   az login
   ```

### Cost Estimates

#### GitHub Container Registry
- **Public repos**: Free unlimited
- **Private repos**: Free for 500MB storage
- Bandwidth: Free for public repos

#### Azure Container Apps (Free Tier)
- **Cost**: $0/month within free tier limits
- **Limits**: 180,000 vCPU-seconds + 360,000 GiB-seconds per month
- **Suitable for**: Multiple hours of testing per day

#### Azure Web App (F1 Free Tier)
- **Cost**: $0/month
- **Limits**: 60 CPU minutes/day, 1GB RAM
- **Suitable for**: Light testing and demos

#### Self-Hosted Docker
- **Cost**: $0 (uses your own infrastructure)
- **Requirements**: Docker installed on any machine

## Workflow Integration with PRs

When you run the manual deployment workflow from a PR branch:

1. The workflow builds and pushes the Docker image
2. A comment is automatically added to the PR with:
   - Docker image location
   - Quick deployment commands
   - Link to this deployment guide

3. Anyone with access to the repository can pull and run the image for testing

## Security Notes

1. **GitHub Container Registry Authentication**:
   - Public repos: No authentication needed to pull images
   - Private repos: Requires GitHub Personal Access Token with `read:packages` scope

2. **Production Considerations**:
   - This setup is for **testing/development only**
   - For production, implement:
     - Proper database (not in-memory)
     - Environment-specific configurations
     - Security hardening
     - SSL/TLS certificates
     - Monitoring and logging
     - Backup strategies

3. **Secrets Management**:
   - Never commit secrets to the repository
   - Use Azure Key Vault or GitHub Secrets for production
   - Use environment variables for configuration

## Troubleshooting

### Image Pull Failures

**Problem**: Cannot pull image from GHCR

**Solution**:
```bash
# For private repos, authenticate first
echo $GITHUB_TOKEN | docker login ghcr.io -u YOUR_USERNAME --password-stdin
```

### Container Starts But App Not Accessible

**Problem**: Container runs but cannot access app

**Solution**:
1. Check container logs: `docker logs mealfortoday-test`
2. Verify port mapping: `-p 8080:8080`
3. Check if port 8080 is already in use: `lsof -i :8080`
4. Try a different port: `-p 3000:8080`

### Azure Deployment Fails

**Problem**: Azure Container Apps or Web App creation fails

**Solution**:
1. Verify Azure CLI is logged in: `az account show`
2. Check resource group exists: `az group list`
3. Ensure unique names (especially for Web Apps)
4. Verify subscription has available quota

### Out of Memory or CPU Limits

**Problem**: App crashes or is slow on free tier

**Solution**:
1. For Azure Container Apps: Increase `--cpu` and `--memory` (may incur costs)
2. For Azure Web App: Upgrade from F1 to B1 tier (paid)
3. Use local Docker deployment for intensive testing

## Next Steps

1. **Try it out**: Run the manual deployment workflow from your PR branch
2. **Test locally**: Pull and run the Docker image on your machine
3. **Deploy to cloud**: Set up Azure Container Apps for shared testing environment
4. **Provide feedback**: Improve this workflow based on your experience

## Support

For issues or questions:
1. Check workflow logs in GitHub Actions
2. Review container logs: `docker logs <container-name>`
3. Check Azure portal for cloud deployment issues
4. Open an issue in the repository
