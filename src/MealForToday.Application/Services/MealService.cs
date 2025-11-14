using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MealForToday.Application.Models;
using MealForToday.Application.Repositories;

namespace MealForToday.Application.Services
{
    public class MealService : IMealService
    {
        private readonly IMealRepository _mealRepo;

        public MealService(IMealRepository mealRepo)
        {
            _mealRepo = mealRepo;
        }

        public Task CreateAsync(Meal meal)
        {
            return _mealRepo.AddAsync(meal);
        }

        public Task DeleteAsync(Guid id)
        {
            return _mealRepo.DeleteAsync(id);
        }

        public Task<List<Meal>> GetAllAsync()
        {
            return _mealRepo.GetAllAsync();
        }

        public Task<Meal?> GetByIdAsync(Guid id)
        {
            return _mealRepo.GetByIdAsync(id);
        }

        public Task UpdateAsync(Meal meal)
        {
            meal.LastModifiedDate = DateTime.UtcNow;
            return _mealRepo.UpdateAsync(meal);
        }
    }
}
