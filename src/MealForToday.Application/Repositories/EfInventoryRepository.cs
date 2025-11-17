using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MealForToday.Application.Models;

namespace MealForToday.Application.Repositories
{
    /// <summary>
    /// Generic EF Core repository implementing the Inventory archetype pattern.
    /// Provides reusable CRUD operations with soft delete support for any InventoryItem.
    /// Can be used directly or inherited by specific repository implementations.
    /// </summary>
    /// <typeparam name="T">The inventory item type</typeparam>
    public class EfInventoryRepository<T> : IInventoryRepository<T> where T : InventoryItem
    {
        protected readonly ApplicationDbContext _db;
        private readonly Func<ApplicationDbContext, DbSet<T>> _dbSetAccessor;

        public EfInventoryRepository(ApplicationDbContext db, Func<ApplicationDbContext, DbSet<T>> dbSetAccessor)
        {
            _db = db;
            _dbSetAccessor = dbSetAccessor;
        }

        /// <summary>
        /// Gets the DbSet for the specific inventory item type
        /// </summary>
        protected virtual DbSet<T> DbSet => _dbSetAccessor(_db);

        public virtual async Task<T?> GetByIdAsync(Guid id)
        {
            return await DbSet.FindAsync(id);
        }

        public virtual async Task<List<T>> GetAllAsync()
        {
            return await DbSet.AsNoTracking().ToListAsync();
        }

        public virtual async Task<List<T>> GetAllIncludingDeletedAsync()
        {
            return await DbSet.IgnoreQueryFilters().AsNoTracking().ToListAsync();
        }

        public virtual async Task AddAsync(T item)
        {
            DbSet.Add(item);
            await _db.SaveChangesAsync();
        }

        public virtual async Task UpdateAsync(T item)
        {
            item.UpdateModificationTracking();
            DbSet.Update(item);
            await _db.SaveChangesAsync();
        }

        public virtual async Task DeleteAsync(Guid id)
        {
            var entity = await DbSet.FindAsync(id);
            if (entity != null)
            {
                entity.MarkAsDeleted();
                await _db.SaveChangesAsync();
            }
        }

        public virtual async Task HardDeleteAsync(Guid id)
        {
            var entity = await DbSet.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == id);
            if (entity != null)
            {
                DbSet.Remove(entity);
                await _db.SaveChangesAsync();
            }
        }

        public virtual async Task RestoreAsync(Guid id)
        {
            var entity = await DbSet.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == id);
            if (entity != null && entity.IsDeleted)
            {
                entity.IsDeleted = false;
                entity.DeletedAt = null;
                entity.DeletedBy = null;
                entity.UpdateModificationTracking();
                await _db.SaveChangesAsync();
            }
        }
    }
}
