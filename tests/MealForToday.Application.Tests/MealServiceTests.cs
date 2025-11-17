using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Xunit;
using MealForToday.Application.Models;
using MealForToday.Application.Repositories;
using MealForToday.Application.Services;

namespace MealForToday.Application.Tests
{
    public class MealServiceTests
    {
        private ApplicationDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            return new ApplicationDbContext(options);
        }

        [Fact]
        public async Task CanCreateAndRetrieveMeal()
        {
            using var db = CreateContext();
            var mealRepo = new EfMealRepository(db);
            var service = new MealService(mealRepo);

            var meal = new Meal { Name = "Test Meal" };
            await service.CreateAsync(meal);

            var all = await service.GetAllAsync();
            Assert.Single(all);
            Assert.Equal("Test Meal", all[0].Name);
        }

        [Fact]
        public async Task MealTracksAuthorId()
        {
            using var db = CreateContext();
            var mealRepo = new EfMealRepository(db);
            var service = new MealService(mealRepo);

            var meal = new Meal
            {
                Name = "Test Meal",
                AuthorId = "user123"
            };
            await service.CreateAsync(meal);

            var retrieved = await service.GetByIdAsync(meal.Id);
            Assert.NotNull(retrieved);
            Assert.Equal("user123", retrieved.AuthorId);
        }

        [Fact]
        public async Task MealWithIngredientsCanBeComposed()
        {
            using var db = CreateContext();
            var ingredientRepo = new EfInventoryRepository<Ingredient>(db, ctx => ctx.Ingredients);
            var mealRepo = new EfMealRepository(db);
            var ingredientService = new IngredientService(ingredientRepo);
            var mealService = new MealService(mealRepo);

            // Create ingredients
            var flour = new Ingredient
            {
                Name = "Flour",
                DefaultUnit = "g"
            };
            var sugar = new Ingredient
            {
                Name = "Sugar",
                DefaultUnit = "g"
            };
            await ingredientService.CreateAsync(flour);
            await ingredientService.CreateAsync(sugar);

            // Create meal with ingredients
            var meal = new Meal
            {
                Name = "Cake",
                AuthorId = "chef123",
                Ingredients = new List<MealIngredient>
                {
                    new MealIngredient
                    {
                        IngredientId = flour.Id,
                        Quantity = 500,
                        Unit = "g",
                        Notes = "All-purpose flour"
                    },
                    new MealIngredient
                    {
                        IngredientId = sugar.Id,
                        Quantity = 200,
                        Unit = "g"
                    }
                }
            };
            await mealService.CreateAsync(meal);

            var retrieved = await mealService.GetByIdAsync(meal.Id);
            Assert.NotNull(retrieved);
            Assert.Equal("Cake", retrieved.Name);
            Assert.Equal("chef123", retrieved.AuthorId);
            Assert.Equal(2, retrieved.Ingredients.Count);
            Assert.Contains(retrieved.Ingredients, i => i.Quantity == 500);
            Assert.Contains(retrieved.Ingredients, i => i.Quantity == 200);
        }

        [Fact]
        public async Task ScheduleServiceGeneratesEntries()
        {
            using var db = CreateContext();
            var mealRepo = new EfMealRepository(db);
            var scheduleRepo = new EfScheduleRepository(db);
            var mealService = new MealService(mealRepo);
            var scheduleService = new ScheduleService(mealRepo, scheduleRepo);

            await mealService.CreateAsync(new Meal { Name = "A" });
            await mealService.CreateAsync(new Meal { Name = "B" });

            var schedule = await scheduleService.GenerateScheduleAsync(DateTime.Today, DateTime.Today.AddDays(2), 2);
            Assert.NotNull(schedule);
            Assert.Equal(3 * 2, schedule.ScheduleEntries.Count);
        }
    }
}
