using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MealForToday.Application.Models;

namespace MealForToday.Application.Repositories
{
    public class EfIngredientRepository : IIngredientRepository
    {
        private readonly ApplicationDbContext _db;

        public EfIngredientRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task AddAsync(Ingredient ingredient)
        {
            _db.Ingredients.Add(ingredient);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var e = await _db.Ingredients.FindAsync(id);
            if (e != null)
            {
                _db.Ingredients.Remove(e);
                await _db.SaveChangesAsync();
            }
        }

        public async Task<List<Ingredient>> GetAllAsync()
        {
            return await _db.Ingredients.AsNoTracking().ToListAsync();
        }

        public async Task<Ingredient?> GetByIdAsync(Guid id)
        {
            return await _db.Ingredients.FindAsync(id);
        }

        public async Task UpdateAsync(Ingredient ingredient)
        {
            _db.Ingredients.Update(ingredient);
            await _db.SaveChangesAsync();
        }
    }
}
