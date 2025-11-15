# Copilot Instructions for MealForToday

## Project Overview
MealForToday is an ASP.NET Core 9.0 Blazor Server application that suggests meals for today. The application uses ASP.NET Core Identity for authentication and Entity Framework Core with SQL Server for data persistence.

## Technology Stack
- **Framework**: .NET 9.0
- **UI**: Blazor Server with Interactive Server Components
- **Authentication**: ASP.NET Core Identity
- **Database**: Entity Framework Core with SQL Server
- **Testing**: xUnit (MealForToday.Application.Tests)

## Project Structure
```
MealForToday/
├── src/
│   ├── MealForToday.Application/     # Application/business logic layer
│   └── MealForToday.UI/               # Blazor Server UI project
│       ├── Components/                # Razor components
│       ├── Data/                      # DbContext and Identity models
│       └── wwwroot/                   # Static files
└── tests/
    └── MealForToday.Application.Tests/  # Unit tests for application layer
```

## Coding Standards
- **Nullable Reference Types**: Enabled across all projects - always use nullable annotations
- **Implicit Usings**: Enabled - common namespaces are automatically imported
- **C# Version**: Latest (C# 13 with .NET 9.0)
- **Code Style**: Follow standard .NET conventions

## Build and Test
```bash
# Restore dependencies
dotnet restore

# Build the solution
dotnet build --configuration Release --no-restore

# Run tests
dotnet test --configuration Release --no-build --verbosity normal
```

## Database
- The application uses Entity Framework Core migrations for database schema management
- Connection strings are stored in `appsettings.json` and `appsettings.Development.json`
- Sensitive configuration should use User Secrets (configured with UserSecretsId)

## Authentication
- ASP.NET Core Identity is configured for user authentication
- Email confirmation is required for account signup
- The application uses cookie-based authentication

## Important Conventions
1. Place business logic in the `MealForToday.Application` project
2. UI components should be in the `MealForToday.UI/Components` directory
3. Database models and migrations go in `MealForToday.UI/Data`
4. Always write tests for new business logic in the Application layer

## CI/CD
- Pull requests trigger automated build and test workflows
- All PRs must pass build and test checks before merging
- Workflow configuration: `.github/workflows/build-and-test.yml`
