using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MealForToday.Application.Models;

namespace MealForToday.Application.Repositories
{
    public class EfUnitDefinitionRepository : IUnitDefinitionRepository
    {
        private readonly ApplicationDbContext _db;

        public EfUnitDefinitionRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task AddAsync(UnitDefinition unitDefinition)
        {
            _db.UnitDefinitions.Add(unitDefinition);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var unit = await _db.UnitDefinitions.FindAsync(id);
            if (unit != null)
            {
                _db.UnitDefinitions.Remove(unit);
                await _db.SaveChangesAsync();
            }
        }

        public async Task<List<UnitDefinition>> GetAllAsync()
        {
            return await _db.UnitDefinitions.AsNoTracking().ToListAsync();
        }

        public async Task<UnitDefinition?> GetByCodeAsync(string code)
        {
            return await _db.UnitDefinitions.AsNoTracking().FirstOrDefaultAsync(x => x.Code == code);
        }

        public async Task<UnitDefinition?> GetByIdAsync(Guid id)
        {
            return await _db.UnitDefinitions.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task UpdateAsync(UnitDefinition unitDefinition)
        {
            _db.UnitDefinitions.Update(unitDefinition);
            await _db.SaveChangesAsync();
        }
    }
}
