using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MealForToday.Application.Models;

namespace MealForToday.Application.Repositories
{
    /// <summary>
    /// Repository interface for Ingredient entities.
    /// Implements the Inventory archetype pattern through IInventoryRepository.
    /// </summary>
    public interface IIngredientRepository : IInventoryRepository<Ingredient>
    {
        // Additional ingredient-specific methods can be added here if needed
    }
}
