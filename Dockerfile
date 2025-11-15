# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy solution and project files
COPY MealForToday.slnx .
COPY src/MealForToday.Application/MealForToday.Application.csproj src/MealForToday.Application/
COPY src/MealForToday.UI/MealForToday.UI.csproj src/MealForToday.UI/

# Restore dependencies
RUN dotnet restore src/MealForToday.UI/MealForToday.UI.csproj

# Copy the rest of the source code
COPY src/ src/

# Build and publish the application
WORKDIR /src/src/MealForToday.UI
RUN dotnet publish -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# Copy published application from build stage
COPY --from=build /app/publish .

# Expose port 8080 (standard for ASP.NET Core in containers)
EXPOSE 8080

# Set environment variables for production
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

# Create a non-root user for security
RUN useradd -m -u 1000 appuser && chown -R appuser:appuser /app
USER appuser

ENTRYPOINT ["dotnet", "MealForToday.UI.dll"]
