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
    public class IngredientServiceTests
    {
        private ApplicationDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            return new ApplicationDbContext(options);
        }

        private IInventoryRepository<Ingredient> CreateRepository(ApplicationDbContext db)
        {
            return new EfInventoryRepository<Ingredient>(db, context => context.Ingredients);
        }

        [Fact]
        public async Task CanCreateAndRetrieveIngredient()
        {
            using var db = CreateContext();
            var repo = CreateRepository(db);
            var service = new IngredientService(repo);

            var ingredient = new Ingredient 
            { 
                Name = "Tomato",
                Description = "Fresh red tomato",
                DefaultUnit = "piece",
                Category = "Vegetable"
            };
            await service.CreateAsync(ingredient);

            var all = await service.GetAllAsync();
            Assert.Single(all);
            Assert.Equal("Tomato", all[0].Name);
            Assert.Equal("Fresh red tomato", all[0].Description);
            Assert.Equal("piece", all[0].DefaultUnit);
        }

        [Fact]
        public async Task SoftDeleteHidesIngredientFromGetAll()
        {
            using var db = CreateContext();
            var repo = CreateRepository(db);
            var service = new IngredientService(repo);

            var ingredient = new Ingredient { Name = "Test Ingredient", DefaultUnit = "g" };
            await service.CreateAsync(ingredient);

            var beforeDelete = await service.GetAllAsync();
            Assert.Single(beforeDelete);

            await service.DeleteAsync(ingredient.Id);

            var afterDelete = await service.GetAllAsync();
            Assert.Empty(afterDelete);
        }

        [Fact]
        public async Task SoftDeletePreservesIngredientData()
        {
            using var db = CreateContext();
            var repo = CreateRepository(db);
            var service = new IngredientService(repo);

            var ingredient = new Ingredient 
            { 
                Name = "Test Ingredient",
                Description = "Test Description",
                DefaultUnit = "kg" 
            };
            await service.CreateAsync(ingredient);

            await service.DeleteAsync(ingredient.Id);

            // Verify the ingredient still exists in database but is marked as deleted
            var deletedIngredient = await db.Ingredients
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(x => x.Id == ingredient.Id);

            Assert.NotNull(deletedIngredient);
            Assert.True(deletedIngredient.IsDeleted);
            Assert.NotNull(deletedIngredient.DeletedAt);
            Assert.Equal("Test Ingredient", deletedIngredient.Name);
            Assert.Equal("Test Description", deletedIngredient.Description);
        }

        [Fact]
        public async Task CanUpdateIngredient()
        {
            using var db = CreateContext();
            var repo = CreateRepository(db);
            var service = new IngredientService(repo);

            var ingredient = new Ingredient 
            { 
                Name = "Original Name",
                Description = "Original Description",
                DefaultUnit = "g"
            };
            await service.CreateAsync(ingredient);

            ingredient.Name = "Updated Name";
            ingredient.Description = "Updated Description";
            ingredient.DefaultUnit = "kg";
            await service.UpdateAsync(ingredient);

            var updated = await service.GetByIdAsync(ingredient.Id);
            Assert.NotNull(updated);
            Assert.Equal("Updated Name", updated.Name);
            Assert.Equal("Updated Description", updated.Description);
            Assert.Equal("kg", updated.DefaultUnit);
        }

        [Fact]
        public async Task GetAllIncludingDeletedReturnsAllIngredients()
        {
            using var db = CreateContext();
            var repo = CreateRepository(db);

            var ingredient1 = new Ingredient { Name = "Active", DefaultUnit = "g" };
            var ingredient2 = new Ingredient { Name = "Deleted", DefaultUnit = "kg" };
            
            await repo.AddAsync(ingredient1);
            await repo.AddAsync(ingredient2);
            await repo.DeleteAsync(ingredient2.Id);

            var active = await repo.GetAllAsync();
            Assert.Single(active);
            Assert.Equal("Active", active[0].Name);

            var all = await repo.GetAllIncludingDeletedAsync();
            Assert.Equal(2, all.Count);
            Assert.Contains(all, i => i.Name == "Active" && !i.IsDeleted);
            Assert.Contains(all, i => i.Name == "Deleted" && i.IsDeleted);
        }

        [Fact]
        public async Task HardDeletePermanentlyRemovesIngredient()
        {
            using var db = CreateContext();
            var repo = CreateRepository(db);

            var ingredient = new Ingredient { Name = "To Be Hard Deleted", DefaultUnit = "ml" };
            await repo.AddAsync(ingredient);

            await repo.HardDeleteAsync(ingredient.Id);

            var all = await repo.GetAllIncludingDeletedAsync();
            Assert.Empty(all);
        }

        [Fact]
        public async Task CanRestoreSoftDeletedIngredient()
        {
            using var db = CreateContext();
            var repo = CreateRepository(db);

            var ingredient = new Ingredient { Name = "To Be Restored", DefaultUnit = "g" };
            await repo.AddAsync(ingredient);

            // Soft delete the ingredient
            await repo.DeleteAsync(ingredient.Id);

            // Verify it's not in active list
            var active = await repo.GetAllAsync();
            Assert.Empty(active);

            // Restore the ingredient
            await repo.RestoreAsync(ingredient.Id);

            // Verify it's back in active list
            var restored = await repo.GetAllAsync();
            Assert.Single(restored);
            Assert.Equal("To Be Restored", restored[0].Name);
            Assert.False(restored[0].IsDeleted);
            Assert.Null(restored[0].DeletedAt);
        }

        [Fact]
        public async Task InventoryArchetypeTracksCreationTime()
        {
            using var db = CreateContext();
            var repo = CreateRepository(db);

            var beforeCreate = DateTime.UtcNow;
            var ingredient = new Ingredient { Name = "Tracked Ingredient", DefaultUnit = "ml" };
            await repo.AddAsync(ingredient);
            var afterCreate = DateTime.UtcNow;

            var retrieved = await repo.GetByIdAsync(ingredient.Id);
            Assert.NotNull(retrieved);
            Assert.True(retrieved.CreatedAt >= beforeCreate && retrieved.CreatedAt <= afterCreate);
        }

        [Fact]
        public async Task InventoryArchetypeTracksModificationTime()
        {
            using var db = CreateContext();
            var repo = CreateRepository(db);

            var ingredient = new Ingredient { Name = "Original", DefaultUnit = "g" };
            await repo.AddAsync(ingredient);

            // Wait a tiny bit to ensure timestamp difference
            await Task.Delay(10);

            ingredient.Name = "Modified";
            await repo.UpdateAsync(ingredient);

            var retrieved = await repo.GetByIdAsync(ingredient.Id);
            Assert.NotNull(retrieved);
            Assert.NotNull(retrieved.LastModifiedAt);
            Assert.True(retrieved.LastModifiedAt > retrieved.CreatedAt);
        }

        [Fact]
        public async Task InventoryArchetypeTracksDeletionTime()
        {
            using var db = CreateContext();
            var repo = CreateRepository(db);

            var ingredient = new Ingredient { Name = "To Delete", DefaultUnit = "kg" };
            await repo.AddAsync(ingredient);

            var beforeDelete = DateTime.UtcNow;
            await repo.DeleteAsync(ingredient.Id);
            var afterDelete = DateTime.UtcNow;

            var deleted = await db.Ingredients
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(x => x.Id == ingredient.Id);

            Assert.NotNull(deleted);
            Assert.True(deleted.IsDeleted);
            Assert.NotNull(deleted.DeletedAt);
            Assert.True(deleted.DeletedAt >= beforeDelete && deleted.DeletedAt <= afterDelete);
        }
    }
}
