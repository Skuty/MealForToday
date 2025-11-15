using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MealForToday.Application.Models;
using MealForToday.Application.Repositories;

namespace MealForToday.Application.Services
{
    public class ScheduleService : IScheduleService
    {
        private readonly IMealRepository _mealRepo;
        private readonly IScheduleRepository _scheduleRepo;
        private readonly Random _rand = new Random();

        public ScheduleService(IMealRepository mealRepo, IScheduleRepository scheduleRepo)
        {
            _mealRepo = mealRepo;
            _scheduleRepo = scheduleRepo;
        }

        public async Task<List<MealSchedule>> GetAllAsync()
        {
            return await _scheduleRepo.GetAllAsync();
        }

        public Task<MealSchedule?> GetByIdAsync(Guid id)
        {
            return _scheduleRepo.GetByIdAsync(id);
        }

        public Task SaveAsync(MealSchedule schedule)
        {
            return _scheduleRepo.AddAsync(schedule);
        }

        public async Task<MealSchedule> GenerateScheduleAsync(DateTime start, DateTime end, int mealsPerDay)
        {
            if (end < start) throw new ArgumentException("End date must be >= start date");

            var meals = await _mealRepo.GetAllAsync();
            if (!meals.Any())
            {
                return new MealSchedule { StartDate = start, EndDate = end };
            }

            var schedule = new MealSchedule
            {
                StartDate = start,
                EndDate = end,
                CreatedDate = DateTime.UtcNow
            };

            var totalDays = (end.Date - start.Date).Days + 1;
            Meal? lastPicked = null;

            for (int d = 0; d < totalDays; d++)
            {
                var date = start.Date.AddDays(d);
                for (int m = 0; m < mealsPerDay; m++)
                {
                    var candidate = PickRandomMealAvoidRepeat(meals, lastPicked);
                    lastPicked = candidate;

                    schedule.ScheduleEntries.Add(new ScheduleEntry
                    {
                        MealId = candidate.Id,
                        Meal = candidate,
                        Date = date,
                        MealType = (MealType)(m % Enum.GetValues(typeof(MealType)).Length)
                    });
                }
            }

            return schedule;
        }

        private Meal PickRandomMealAvoidRepeat(List<Meal> meals, Meal? lastPicked)
        {
            if (meals.Count == 1) return meals[0];
            Meal candidate;
            var attempts = 0;
            do
            {
                candidate = meals[_rand.Next(meals.Count)];
                attempts++;
            } while (lastPicked != null && candidate.Id == lastPicked.Id && attempts < 10);

            return candidate;
        }
    }
}
