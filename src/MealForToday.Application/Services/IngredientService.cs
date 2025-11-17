using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MealForToday.Application.Models;
using MealForToday.Application.Repositories;

namespace MealForToday.Application.Services
{
    public class IngredientService : IIngredientService
    {
        private readonly IInventoryRepository<Ingredient> _repo;

        public IngredientService(IInventoryRepository<Ingredient> repo)
        {
            _repo = repo;
        }

        public Task CreateAsync(Ingredient ingredient)
        {
            return _repo.AddAsync(ingredient);
        }

        public Task DeleteAsync(Guid id)
        {
            return _repo.DeleteAsync(id);
        }

        public Task<List<Ingredient>> GetAllAsync()
        {
            return _repo.GetAllAsync();
        }

        public Task<Ingredient?> GetByIdAsync(Guid id)
        {
            return _repo.GetByIdAsync(id);
        }

        public Task UpdateAsync(Ingredient ingredient)
        {
            return _repo.UpdateAsync(ingredient);
        }
    }
}
