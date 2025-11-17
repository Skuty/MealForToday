using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MealForToday.Application.Models;

namespace MealForToday.Application.Services
{
    public interface IUnitDefinitionService
    {
        Task<List<UnitDefinition>> GetAllAsync();
        Task<UnitDefinition?> GetByIdAsync(Guid id);
        Task<UnitDefinition?> GetByCodeAsync(string code);
        Task CreateAsync(UnitDefinition unitDefinition);
        Task UpdateAsync(UnitDefinition unitDefinition);
        Task DeleteAsync(Guid id);
        Task SeedStandardUnitsAsync();
    }
}
