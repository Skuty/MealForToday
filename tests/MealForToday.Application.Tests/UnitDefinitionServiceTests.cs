using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Xunit;
using MealForToday.Application.Models;
using MealForToday.Application.Repositories;
using MealForToday.Application.Services;

namespace MealForToday.Application.Tests
{
    public class UnitDefinitionServiceTests
    {
        private ApplicationDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            return new ApplicationDbContext(options);
        }

        [Fact]
        public async Task CanCreateAndRetrieveUnitDefinition()
        {
            using var db = CreateContext();
            var repo = new EfInventoryRepository<UnitDefinition>(db, ctx => ctx.UnitDefinitions);
            var service = new UnitDefinitionService(repo);

            var unit = new UnitDefinition
            {
                Code = "kg",
                Name = "Kilogram",
                BaseAmount = 1,
                MeasurementType = "Weight",
                IsStandard = true
            };

            await repo.AddAsync(unit);

            var all = await repo.GetAllAsync();
            Assert.Single(all);
            Assert.Equal("Kilogram", all[0].Name);
            Assert.Equal(1, all[0].BaseAmount);
        }

        [Fact]
        public async Task CanGetUnitByCode()
        {
            using var db = CreateContext();
            var repo = new EfInventoryRepository<UnitDefinition>(db, ctx => ctx.UnitDefinitions);
            var service = new UnitDefinitionService(repo);

            var unit = new UnitDefinition
            {
                Code = "g",
                Name = "Gram",
                BaseAmount = 1000,
                MeasurementType = "Weight"
            };

            await repo.AddAsync(unit);

            var retrieved = await service.GetByCodeAsync("g");
            Assert.NotNull(retrieved);
            Assert.Equal("Gram", retrieved.Name);
            Assert.Equal(1000, retrieved.BaseAmount);
        }

        [Fact]
        public async Task SeedStandardUnitsCreatesExpectedUnits()
        {
            using var db = CreateContext();
            var repo = new EfInventoryRepository<UnitDefinition>(db, ctx => ctx.UnitDefinitions);
            var service = new UnitDefinitionService(repo);

            await service.SeedStandardUnitsAsync();

            var all = await repo.GetAllAsync();
            Assert.True(all.Count >= 4); // At least kg, g, l, ml

            var kg = await service.GetByCodeAsync("kg");
            Assert.NotNull(kg);
            Assert.Equal(1, kg.BaseAmount);

            var g = await service.GetByCodeAsync("g");
            Assert.NotNull(g);
            Assert.Equal(1000, g.BaseAmount);

            var l = await service.GetByCodeAsync("l");
            Assert.NotNull(l);
            Assert.Equal(1, l.BaseAmount);

            var ml = await service.GetByCodeAsync("ml");
            Assert.NotNull(ml);
            Assert.Equal(1000, ml.BaseAmount);
        }

        [Fact]
        public async Task SeedStandardUnitsDoesNotDuplicateExistingUnits()
        {
            using var db = CreateContext();
            var repo = new EfInventoryRepository<UnitDefinition>(db, ctx => ctx.UnitDefinitions);
            var service = new UnitDefinitionService(repo);

            await service.SeedStandardUnitsAsync();
            var countAfterFirstSeed = (await repo.GetAllAsync()).Count;

            await service.SeedStandardUnitsAsync();
            var countAfterSecondSeed = (await repo.GetAllAsync()).Count;

            Assert.Equal(countAfterFirstSeed, countAfterSecondSeed);
        }

        [Fact]
        public async Task CanUpdateUnitDefinition()
        {
            using var db = CreateContext();
            var repo = new EfInventoryRepository<UnitDefinition>(db, ctx => ctx.UnitDefinitions);
            var service = new UnitDefinitionService(repo);

            var unit = new UnitDefinition
            {
                Code = "test",
                Name = "Test Unit",
                BaseAmount = 1
            };

            await repo.AddAsync(unit);

            unit.Name = "Updated Test Unit";
            unit.BaseAmount = 2;
            await repo.UpdateAsync(unit);

            var retrieved = await repo.GetByIdAsync(unit.Id);
            Assert.NotNull(retrieved);
            Assert.Equal("Updated Test Unit", retrieved.Name);
            Assert.Equal(2, retrieved.BaseAmount);
        }

        [Fact]
        public async Task CanSoftDeleteUnitDefinition()
        {
            using var db = CreateContext();
            var repo = new EfInventoryRepository<UnitDefinition>(db, ctx => ctx.UnitDefinitions);
            var service = new UnitDefinitionService(repo);

            var unit = new UnitDefinition
            {
                Code = "temp",
                Name = "Temporary Unit",
                BaseAmount = 1
            };

            await repo.AddAsync(unit);
            var id = unit.Id;

            // Soft delete
            await repo.DeleteAsync(id);

            // Should not be in GetAll (query filter applies)
            var all = await repo.GetAllAsync();
            Assert.DoesNotContain(all, u => u.Id == id);

            // Can still be retrieved with GetAllIncludingDeleted
            var allIncludingDeleted = await repo.GetAllIncludingDeletedAsync();
            var deletedUnit = allIncludingDeleted.FirstOrDefault(u => u.Id == id);
            Assert.NotNull(deletedUnit);
            Assert.True(deletedUnit.IsDeleted);
        }
    }
}
