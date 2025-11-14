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

