using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MealForToday.Application.Models;

namespace MealForToday.Application.Repositories
{
    public interface IUnitDefinitionRepository
    {
        Task<UnitDefinition?> GetByIdAsync(Guid id);
        Task<UnitDefinition?> GetByCodeAsync(string code);
        Task<List<UnitDefinition>> GetAllAsync();
        Task AddAsync(UnitDefinition unitDefinition);
        Task UpdateAsync(UnitDefinition unitDefinition);
        Task DeleteAsync(Guid id);
    }
}
