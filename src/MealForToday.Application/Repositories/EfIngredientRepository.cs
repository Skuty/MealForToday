using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MealForToday.Application.Models;

namespace MealForToday.Application.Repositories
{
    /// <summary>
    /// EF Core repository for Ingredient entities.
    /// Inherits from EfInventoryRepository to leverage the Inventory archetype pattern.
    /// </summary>
    public class EfIngredientRepository : EfInventoryRepository<Ingredient>, IIngredientRepository
    {
        public EfIngredientRepository(ApplicationDbContext db) : base(db)
        {
        }

        protected override DbSet<Ingredient> DbSet => _db.Ingredients;

        // Additional ingredient-specific methods can be added here if needed
    }
}
