# Quick Deployment Guide for Developers

This is a quick reference for deploying your PR branch for testing.

## TL;DR - Deploy My PR to Azure (Recommended)

**One-time setup**: Follow [AZURE-SETUP.md](AZURE-SETUP.md) to configure Azure authentication.

1. **Deploy**:
   - Go to [Actions → Manual Deploy to Test Environment](../../actions/workflows/manual-deploy.yml)
   - Click "Run workflow"
   - Configure:
     - Action: `deploy`
     - Environment: `test`
     - Deploy to Azure: ✅ (checked)
     - Branch: Your PR branch
   - Click "Run workflow"

2. **Wait for Deployment** (~3-5 minutes):
   - Workflow builds and deploys to Azure
   - Auto-configured with HTTPS endpoint
   - Auto-scales to 0 when idle (saves costs)

3. **Access Your App**:
   - Get URL from workflow summary or PR comment
   - Example: `https://mealfortoday-test-mybranch.azurecontainerapps.io`
   - Use demo credentials (see below)

4. **Cleanup When Done**:
   - Run workflow again with Action: `cleanup`
   - Removes all Azure resources for your branch

## Alternative: Deploy Locally with Docker

1. **Trigger Build**:
   - Go to [Actions → Manual Deploy to Test Environment](../../actions/workflows/manual-deploy.yml)
   - Click "Run workflow"
   - Configure:
     - Action: `deploy`
     - Environment: `test`
     - Deploy to Azure: ❌ (unchecked)
     - Branch: Your PR branch
   - Click "Run workflow"

2. **Wait for Build** (~3-5 minutes):
   - Workflow builds Docker image
   - Pushes to GitHub Container Registry
   - Posts comment on your PR with instructions

3. **Test Locally** (requires Docker):
   ```bash
   docker pull ghcr.io/skuty/mealfortoday:YOUR-BRANCH-NAME
   docker run -d -p 8080:8080 ghcr.io/skuty/mealfortoday:YOUR-BRANCH-NAME
   ```
   Open http://localhost:8080

4. **Or Share for Testing**:
   - Share the docker pull/run commands from step 3
   - Anyone with Docker can test your changes

## When to Use This

✅ **Use automated Azure deployment when:**
- Want quick cloud testing without Docker setup
- Sharing with non-technical reviewers (just send URL)
- Testing from mobile/tablet devices
- Need HTTPS endpoint for testing
- Want auto-scaling and cost optimization

✅ **Use local Docker deployment when:**
- Testing locally before sharing
- Don't have Azure configured yet
- Prefer local control
- Testing offline scenarios

❌ **Don't use deployment when:**
- Making simple code changes (use Codespaces or local dev)
- Running unit tests (use `dotnet test` locally or in Codespaces)
- First time testing - try local `dotnet run` first

## Workflow Permissions

The workflow needs:
- ✅ Read access to repository code
- ✅ Write access to GitHub Packages (GHCR)
- ✅ Write access to Pull Requests (for comments)

These are configured in the workflow and should work automatically.

## What Gets Built

The workflow:
1. Checks out your branch
2. Builds .NET application in Docker
3. Creates optimized production image
4. Pushes to `ghcr.io/skuty/mealfortoday:YOUR-BRANCH-NAME`
5. Tags with branch name and commit SHA

## Demo Credentials

When testing the deployed app:
- **Email**: `demo@mealfortoday.local`
- **Username**: `demo`
- **Password**: `Password123!`

Note: The app uses in-memory database, so data is lost when container restarts.

## Troubleshooting

**Q: Workflow failed to build**
- Check workflow logs for build errors
- Ensure your branch builds locally: `dotnet build`
- Verify all dependencies are in .csproj files

**Q: Cannot pull image**
- For public repo: No auth needed
- For private repo: Need GitHub token with `read:packages` scope
  ```bash
  echo $GITHUB_TOKEN | docker login ghcr.io -u USERNAME --password-stdin
  ```

**Q: Container starts but app doesn't work**
- Check container logs: `docker logs <container-name>`
- Verify port 8080 is accessible
- Check if another process is using port 8080

**Q: Want to deploy to cloud instead of locally**
- See [DEPLOYMENT.md](DEPLOYMENT.md) for Azure options
- Free tiers available for Azure Container Apps and Web Apps

## Advanced Usage

### Multiple Environments

The workflow supports different environments:
- `test`: For PR testing (default)
- `staging`: For pre-production testing

### Custom Deployment

Pull the image and run with custom settings:
```bash
docker run -d \
  --name my-test-app \
  -p 3000:8080 \
  -e ASPNETCORE_ENVIRONMENT=Development \
  ghcr.io/skuty/mealfortoday:YOUR-BRANCH-NAME
```

### Check Running Containers

```bash
# List running containers
docker ps

# View logs
docker logs -f <container-name>

# Stop container
docker stop <container-name>

# Remove container
docker rm <container-name>
```

## Cost

Everything is **FREE** when using:
- ✅ GitHub Container Registry (public repos)
- ✅ GitHub Actions (2000 minutes/month free tier)
- ✅ Local Docker deployment

Optional cloud deployment costs:
- Azure Container Apps: Free tier available (180k vCPU-seconds/month)
- Azure Web App: Free F1 tier available

## More Info

See [DEPLOYMENT.md](DEPLOYMENT.md) for:
- Detailed deployment instructions
- Cloud deployment options (Azure)
- Infrastructure setup
- Security considerations
