using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MealForToday.Application.Models;

namespace MealForToday.Application.Repositories
{
    public interface IIngredientRepository
    {
        Task<Ingredient?> GetByIdAsync(Guid id);
        Task<List<Ingredient>> GetAllAsync();
        Task AddAsync(Ingredient ingredient);
        Task UpdateAsync(Ingredient ingredient);
        Task DeleteAsync(Guid id);
    }
}
