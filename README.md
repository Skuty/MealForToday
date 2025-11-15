# MealForToday
Meal planning application (MVP) — models, services, Blazor UI and tests.

## Build and run (development)

1. Ensure .NET 9 SDK is installed.
2. From repository root run:

```bash
dotnet build
dotnet run --project src/MealForToday.UI/MealForToday.UI.csproj
```

The app defaults to using the `DefaultConnection` from configuration. If not provided, it falls back to a local SQLite file `mealfortoday.db`.

## Database and migrations

This project uses Entity Framework Core. To create and apply migrations locally:

```bash
dotnet tool install --global dotnet-ef
dotnet ef migrations add InitialCreate --project src/MealForToday.Application --startup-project src/MealForToday.UI
dotnet ef database update --project src/MealForToday.Application --startup-project src/MealForToday.UI
```

If you prefer in-memory or SQLite for development, the `Program.cs` will fall back to SQLite when `DefaultConnection` is not a SQL Server connection string.

## Tests

Run unit tests:

```bash
dotnet test
```

## Deployment (Testing Features from PRs)

You can deploy the application from any PR branch for testing without building locally. See [DEPLOYMENT.md](DEPLOYMENT.md) for complete instructions.

### Automated Azure Deployment (Recommended)

Deploy to Azure Container Apps with auto-scaling to zero:

1. **One-time setup**: Configure Azure auth following [AZURE-SETUP.md](AZURE-SETUP.md)
2. Go to **Actions** → **Manual Deploy to Test Environment**
3. Configure: Action=`deploy`, Deploy to Azure=✅, select your branch
4. Get your app URL (e.g., `https://mealfortoday-test-mybranch.azurecontainerapps.io`)
5. **Cleanup**: Run workflow with Action=`cleanup` to remove all container apps in the environment

**Features:**
- ✅ Auto-scales to 0 when idle (free tier friendly)
- ✅ One-click deployment and cleanup
- ✅ HTTPS endpoint automatically configured

### Manual Docker Deployment

1. Go to **Actions** → **Manual Deploy to Test Environment**
2. Configure: Action=`deploy`, Deploy to Azure=unchecked, select your branch
3. After workflow completes, deploy locally:

```bash
# Pull and run locally
docker pull ghcr.io/skuty/mealfortoday:<your-branch>
docker run -d -p 8080:8080 ghcr.io/skuty/mealfortoday:<your-branch>
```

See [DEPLOYMENT.md](DEPLOYMENT.md) for Azure Container Apps and Azure Web App deployment options.

## What I implemented (MVP)

- Domain models: `Meal`, `Ingredient`, `MealIngredient`, `MealSchedule`, `ScheduleEntry`.
- EF Core `ApplicationDbContext` with DbSets and basic mapping.
- Repositories: `IMealRepository`, `IIngredientRepository`, `IScheduleRepository` with EF implementations.
- Services: `MealService`, `IngredientService`, `ScheduleService` (simple random schedule generator).
- Blazor UI pages: `Pages/Meals.razor` and `Pages/Schedules.razor` for basic CRUD and schedule generation.
- Unit tests using EF Core InMemory for `MealService` and `ScheduleService`.

If you'd like, I can now:

- Add migration scaffolding to the repo and apply it here.
- Expand the UI forms and ingredient autocomplete.
- Add more comprehensive tests and CI configuration.

## Demo credentials (development only)

- A demo Identity user is auto-created on app startup when running in development with the in-memory stores.
- Email: `demo@mealfortoday.local`
- Username: `demo`
- Password: `Password123!`
- Roles: `Admin`, `User` (both roles are created and assigned automatically)

Warning: These credentials are for local development only. Do not use them in production.

