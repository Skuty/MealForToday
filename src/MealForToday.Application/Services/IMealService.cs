using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MealForToday.Application.Models;

namespace MealForToday.Application.Services
{
    public interface IMealService
    {
        Task<List<Meal>> GetAllAsync();
        Task<Meal?> GetByIdAsync(Guid id);
        Task CreateAsync(Meal meal);
        Task UpdateAsync(Meal meal);
        Task DeleteAsync(Guid id);
    }
}
