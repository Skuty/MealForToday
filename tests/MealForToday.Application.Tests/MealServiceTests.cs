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
