using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MealForToday.Application.Models;

namespace MealForToday.Application.Repositories
{
    public interface IMealRepository
    {
        Task<Meal?> GetByIdAsync(Guid id);
        Task<List<Meal>> GetAllAsync();
        Task AddAsync(Meal meal);
        Task UpdateAsync(Meal meal);
        Task DeleteAsync(Guid id);
    }
}
