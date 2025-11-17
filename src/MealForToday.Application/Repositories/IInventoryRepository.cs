using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MealForToday.Application.Models;

namespace MealForToday.Application.Repositories
{
    /// <summary>
    /// Generic repository interface implementing the Inventory archetype pattern.
    /// Provides standard CRUD operations with soft delete support for inventory items.
    /// This interface can be reused for any entity inheriting from InventoryItem.
    /// </summary>
    /// <typeparam name="T">The inventory item type</typeparam>
    public interface IInventoryRepository<T> where T : InventoryItem
    {
        /// <summary>
        /// Retrieves an item by its unique identifier
        /// </summary>
        Task<T?> GetByIdAsync(Guid id);

        /// <summary>
        /// Retrieves all active (non-deleted) items
        /// </summary>
        Task<List<T>> GetAllAsync();

        /// <summary>
        /// Retrieves all items including soft-deleted ones
        /// </summary>
        Task<List<T>> GetAllIncludingDeletedAsync();

        /// <summary>
        /// Adds a new item to the inventory
        /// </summary>
        Task AddAsync(T item);

        /// <summary>
        /// Updates an existing item in the inventory
        /// </summary>
        Task UpdateAsync(T item);

        /// <summary>
        /// Soft deletes an item (marks as deleted but preserves data)
        /// </summary>
        Task DeleteAsync(Guid id);

        /// <summary>
        /// Permanently removes an item from the inventory (hard delete)
        /// Should be used with caution as this operation is irreversible
        /// </summary>
        Task HardDeleteAsync(Guid id);

        /// <summary>
        /// Restores a soft-deleted item back to active status
        /// </summary>
        Task RestoreAsync(Guid id);
    }
}
