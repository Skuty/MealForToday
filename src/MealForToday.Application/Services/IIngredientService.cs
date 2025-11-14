using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MealForToday.Application.Models;

namespace MealForToday.Application.Services
{
    public interface IIngredientService
    {
        Task<List<Ingredient>> GetAllAsync();
        Task<Ingredient?> GetByIdAsync(Guid id);
        Task CreateAsync(Ingredient ingredient);
        Task UpdateAsync(Ingredient ingredient);
        Task DeleteAsync(Guid id);
    }
}
