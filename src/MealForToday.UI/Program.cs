using MealForToday.UI.Components;
using MealForToday.UI.Components.Account;
using MealForToday.UI.Data;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;

namespace MealForToday.UI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();

            // Add MudBlazor services
            builder.Services.AddMudServices();

            builder.Services.AddCascadingAuthenticationState();
            builder.Services.AddScoped<IdentityUserAccessor>();
            builder.Services.AddScoped<IdentityRedirectManager>();
            builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

            builder.Services.AddAuthentication(options =>
                {
                    options.DefaultScheme = IdentityConstants.ApplicationScheme;
                    options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
                })
                .AddIdentityCookies();

            // Use EF Core InMemory provider for both application and identity DbContexts (development/testing convenience).
            builder.Services.AddDbContext<MealForToday.Application.ApplicationDbContext>(options =>
                options.UseInMemoryDatabase("MealForToday.Application"));

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseInMemoryDatabase("MealForToday.UI.Identity"));

            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            builder.Services.AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddRoles<Microsoft.AspNetCore.Identity.IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddSignInManager()
                .AddDefaultTokenProviders();

            builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();

            // Application services and repositories
            builder.Services.AddScoped<MealForToday.Application.Repositories.IMealRepository, MealForToday.Application.Repositories.EfMealRepository>();
            builder.Services.AddScoped<MealForToday.Application.Repositories.IInventoryRepository<MealForToday.Application.Models.Ingredient>>(
                sp => new MealForToday.Application.Repositories.EfInventoryRepository<MealForToday.Application.Models.Ingredient>(
                    sp.GetRequiredService<MealForToday.Application.ApplicationDbContext>(),
                    db => db.Ingredients
                )
            );
            builder.Services.AddScoped<MealForToday.Application.Repositories.IInventoryRepository<MealForToday.Application.Models.UnitDefinition>>(
                sp => new MealForToday.Application.Repositories.EfInventoryRepository<MealForToday.Application.Models.UnitDefinition>(
                    sp.GetRequiredService<MealForToday.Application.ApplicationDbContext>(),
                    db => db.UnitDefinitions
                )
            );
            builder.Services.AddScoped<MealForToday.Application.Repositories.IScheduleRepository, MealForToday.Application.Repositories.EfScheduleRepository>();

            builder.Services.AddScoped<MealForToday.Application.Services.IMealService, MealForToday.Application.Services.MealService>();
            builder.Services.AddScoped<MealForToday.Application.Services.IIngredientService, MealForToday.Application.Services.IngredientService>();
            builder.Services.AddScoped<MealForToday.Application.Services.IScheduleService, MealForToday.Application.Services.ScheduleService>();
            builder.Services.AddScoped<MealForToday.Application.Services.IUnitDefinitionService, MealForToday.Application.Services.UnitDefinitionService>();

            var app = builder.Build();

            // Ensure in-memory databases are created on startup. InMemory provider does not support migrations,
            // so we use EnsureCreated() to initialize the store.
            using (var scope = app.Services.CreateScope())
            {
                var provider = scope.ServiceProvider;
                try
                {
                    var appDb = provider.GetRequiredService<MealForToday.Application.ApplicationDbContext>();
                    appDb.Database.EnsureCreated();

                    // Seed standard unit definitions
                    var unitService = provider.GetRequiredService<MealForToday.Application.Services.IUnitDefinitionService>();
                    unitService.SeedStandardUnitsAsync().GetAwaiter().GetResult();

                    // Seed sample domain data if empty
                    if (!appDb.Meals.Any() && !appDb.Ingredients.Any())
                    {
                        // Ingredients
                        var tomato = new MealForToday.Application.Models.Ingredient { Name = "Tomato", Category = "Vegetable", DefaultUnit = "piece" };
                        var pasta = new MealForToday.Application.Models.Ingredient { Name = "Pasta", Category = "Grain", DefaultUnit = "g" };
                        var cheese = new MealForToday.Application.Models.Ingredient { Name = "Cheese", Category = "Dairy", DefaultUnit = "g" };

                        appDb.Ingredients.AddRange(tomato, pasta, cheese);

                        // Meals
                        var caprese = new MealForToday.Application.Models.Meal
                        {
                            Name = "Caprese Salad",
                            Description = "Tomato, mozzarella and basil salad"
                        };
                        caprese.Ingredients.Add(new MealForToday.Application.Models.MealIngredient { Ingredient = tomato, Quantity = 2, Unit = "piece" });
                        caprese.Ingredients.Add(new MealForToday.Application.Models.MealIngredient { Ingredient = cheese, Quantity = 100, Unit = "g" });

                        var pastaMeal = new MealForToday.Application.Models.Meal
                        {
                            Name = "Simple Pasta",
                            Description = "Pasta with tomato sauce"
                        };
                        pastaMeal.Ingredients.Add(new MealForToday.Application.Models.MealIngredient { Ingredient = pasta, Quantity = 200, Unit = "g" });
                        pastaMeal.Ingredients.Add(new MealForToday.Application.Models.MealIngredient { Ingredient = tomato, Quantity = 3, Unit = "piece" });

                        appDb.Meals.AddRange(caprese, pastaMeal);

                        // Sample schedule
                        var schedule = new MealForToday.Application.Models.MealSchedule
                        {
                            StartDate = DateTime.Today,
                            EndDate = DateTime.Today.AddDays(2),
                        };
                        var meals = new[] { caprese, pastaMeal };
                        var dayCount = (schedule.EndDate - schedule.StartDate).Days + 1;
                        for (int d = 0; d < dayCount; d++)
                        {
                            var date = schedule.StartDate.AddDays(d);
                            for (int m = 0; m < 2; m++)
                            {
                                schedule.ScheduleEntries.Add(new MealForToday.Application.Models.ScheduleEntry
                                {
                                    Date = date,
                                    Meal = meals[(d + m) % meals.Length],
                                    MealType = MealForToday.Application.Models.MealType.Dinner
                                });
                            }
                        }
                        appDb.MealSchedules.Add(schedule);

                        appDb.SaveChanges();
                    }

                    var identityDb = provider.GetRequiredService<ApplicationDbContext>();
                    identityDb.Database.EnsureCreated();

                    // Seed a demo Identity user for development (if none exists)
                    try
                    {
                        var userManager = provider.GetRequiredService<Microsoft.AspNetCore.Identity.UserManager<ApplicationUser>>();
                        var roleManager = provider.GetRequiredService<Microsoft.AspNetCore.Identity.RoleManager<Microsoft.AspNetCore.Identity.IdentityRole>>();

                        var demoEmail = "demo@mealfortoday.local";
                        var demoUser = userManager.FindByEmailAsync(demoEmail).GetAwaiter().GetResult();
                        if (demoUser == null)
                        {
                            demoUser = new ApplicationUser
                            {
                                UserName = "demo",
                                Email = demoEmail,
                                EmailConfirmed = true
                            };
                            var createResult = userManager.CreateAsync(demoUser, "Password123!").GetAwaiter().GetResult();
                            if (!createResult.Succeeded)
                            {
                                var logger = provider.GetService<ILogger<Program>>();
                                logger?.LogWarning("Failed to create demo user: {Errors}", string.Join(',', createResult.Errors.Select(e => e.Description)));
                            }
                        }

                        // Ensure roles exist and assign to demo user
                        var roles = new[] { "Admin", "User" };
                        foreach (var role in roles)
                        {
                            var exists = roleManager.RoleExistsAsync(role).GetAwaiter().GetResult();
                            if (!exists)
                            {
                                var roleResult = roleManager.CreateAsync(new Microsoft.AspNetCore.Identity.IdentityRole(role)).GetAwaiter().GetResult();
                                if (!roleResult.Succeeded)
                                {
                                    var logger = provider.GetService<ILogger<Program>>();
                                    logger?.LogWarning("Failed to create role {Role}: {Errors}", role, string.Join(',', roleResult.Errors.Select(e => e.Description)));
                                }
                            }
                        }

                        // Assign roles to demo user
                        var userRoles = userManager.GetRolesAsync(demoUser).GetAwaiter().GetResult();
                        var missing = roles.Except(userRoles).ToArray();
                        if (missing.Length > 0)
                        {
                            var addRolesResult = userManager.AddToRolesAsync(demoUser, missing).GetAwaiter().GetResult();
                            if (!addRolesResult.Succeeded)
                            {
                                var logger = provider.GetService<ILogger<Program>>();
                                logger?.LogWarning("Failed to assign roles to demo user: {Errors}", string.Join(',', addRolesResult.Errors.Select(e => e.Description)));
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        var logger = provider.GetService<ILogger<Program>>();
                        logger?.LogError(ex, "An error occurred seeding demo Identity user.");
                    }
                }
                catch (Exception ex)
                {
                    var logger = provider.GetService<ILogger<Program>>();
                    logger?.LogError(ex, "An error occurred creating the in-memory databases.");
                }
            }

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            // app.UseHttpsRedirection();

            app.UseAntiforgery();

            app.MapStaticAssets();
            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode();

            // Add additional endpoints required by the Identity /Account Razor components.
            app.MapAdditionalIdentityEndpoints();

            app.Run();
        }
    }
}
