using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MealForToday.Application.Models;
using MealForToday.Application.Repositories;

namespace MealForToday.Application.Services
{
    public class UnitDefinitionService : IUnitDefinitionService
    {
        private readonly IUnitDefinitionRepository _repo;

        public UnitDefinitionService(IUnitDefinitionRepository repo)
        {
            _repo = repo;
        }

        public Task CreateAsync(UnitDefinition unitDefinition)
        {
            return _repo.AddAsync(unitDefinition);
        }

        public Task DeleteAsync(Guid id)
        {
            return _repo.DeleteAsync(id);
        }

        public Task<List<UnitDefinition>> GetAllAsync()
        {
            return _repo.GetAllAsync();
        }

        public Task<UnitDefinition?> GetByCodeAsync(string code)
        {
            return _repo.GetByCodeAsync(code);
        }

        public Task<UnitDefinition?> GetByIdAsync(Guid id)
        {
            return _repo.GetByIdAsync(id);
        }

        public Task UpdateAsync(UnitDefinition unitDefinition)
        {
            return _repo.UpdateAsync(unitDefinition);
        }

        /// <summary>
        /// Seeds standard unit definitions into the database if they don't exist.
        /// Based on requirements: Kilogram=1, Gram=100, Litre=1, Milliliters=1000
        /// </summary>
        public async Task SeedStandardUnitsAsync()
        {
            var standardUnits = new List<UnitDefinition>
            {
                new UnitDefinition
                {
                    Code = "kg",
                    Name = "Kilogram",
                    BaseAmount = 1,
                    MeasurementType = "Weight",
                    IsStandard = true
                },
                new UnitDefinition
                {
                    Code = "g",
                    Name = "Gram",
                    BaseAmount = 1000,
                    MeasurementType = "Weight",
                    IsStandard = true
                },
                new UnitDefinition
                {
                    Code = "l",
                    Name = "Litre",
                    BaseAmount = 1,
                    MeasurementType = "Volume",
                    IsStandard = true
                },
                new UnitDefinition
                {
                    Code = "ml",
                    Name = "Milliliter",
                    BaseAmount = 1000,
                    MeasurementType = "Volume",
                    IsStandard = true
                },
                new UnitDefinition
                {
                    Code = "tbsp",
                    Name = "Tablespoon",
                    BaseAmount = 1,
                    MeasurementType = "Volume",
                    IsStandard = true
                },
                new UnitDefinition
                {
                    Code = "tsp",
                    Name = "Teaspoon",
                    BaseAmount = 3,
                    MeasurementType = "Volume",
                    IsStandard = true
                },
                new UnitDefinition
                {
                    Code = "cup",
                    Name = "Cup",
                    BaseAmount = 1,
                    MeasurementType = "Volume",
                    IsStandard = true
                },
                new UnitDefinition
                {
                    Code = "piece",
                    Name = "Piece",
                    BaseAmount = 1,
                    MeasurementType = "Count",
                    IsStandard = true
                },
                new UnitDefinition
                {
                    Code = "oz",
                    Name = "Ounce",
                    BaseAmount = 1,
                    MeasurementType = "Weight",
                    IsStandard = true
                },
                new UnitDefinition
                {
                    Code = "lb",
                    Name = "Pound",
                    BaseAmount = 1,
                    MeasurementType = "Weight",
                    IsStandard = true
                }
            };

            foreach (var unit in standardUnits)
            {
                var existing = await _repo.GetByCodeAsync(unit.Code);
                if (existing == null)
                {
                    await _repo.AddAsync(unit);
                }
            }
        }
    }
}
