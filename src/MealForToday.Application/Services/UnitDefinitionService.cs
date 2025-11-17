using System.Linq;
using System.Threading.Tasks;
using MealForToday.Application.Models;
using MealForToday.Application.Repositories;

namespace MealForToday.Application.Services
{
    public class UnitDefinitionService : IUnitDefinitionService
    {
        private readonly IInventoryRepository<UnitDefinition> _repo;

        public UnitDefinitionService(IInventoryRepository<UnitDefinition> repo)
        {
            _repo = repo;
        }

        public async Task<UnitDefinition?> GetByCodeAsync(string code)
        {
            var units = await _repo.GetAllAsync();
            return units.FirstOrDefault(u => u.Code == code);
        }

        /// <summary>
        /// Seeds standard unit definitions into the database if they don't exist.
        /// Based on requirements: Kilogram=1, Gram=1000, Litre=1, Milliliters=1000
        /// </summary>
        public async Task SeedStandardUnitsAsync()
        {
            var standardUnits = new[]
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
                var existing = await GetByCodeAsync(unit.Code);
                if (existing == null)
                {
                    await _repo.AddAsync(unit);
                }
            }
        }
    }
}
