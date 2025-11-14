using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MealForToday.Application.Models;

namespace MealForToday.Application.Repositories
{
    public class EfMealRepository : IMealRepository
    {
        private readonly ApplicationDbContext _db;

        public EfMealRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task AddAsync(Meal meal)
        {
            _db.Meals.Add(meal);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var m = await _db.Meals.FindAsync(id);
            if (m != null)
            {
                _db.Meals.Remove(m);
                await _db.SaveChangesAsync();
            }
        }

        public async Task<List<Meal>> GetAllAsync()
        {
            return await _db.Meals.AsNoTracking().ToListAsync();
        }

        public async Task<Meal?> GetByIdAsync(Guid id)
        {
            return await _db.Meals.Include(x => x.Ingredients).FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task UpdateAsync(Meal meal)
        {
            _db.Meals.Update(meal);
            await _db.SaveChangesAsync();
        }
    }
}
