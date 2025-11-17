using System.Threading.Tasks;
using MealForToday.Application.Models;

namespace MealForToday.Application.Services
{
    public interface IUnitDefinitionService
    {
        Task<UnitDefinition?> GetByCodeAsync(string code);
        Task SeedStandardUnitsAsync();
    }
}
