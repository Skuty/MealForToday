using System;

namespace MealForToday.Application.Models
{
    /// <summary>
    /// Base class implementing the Inventory archetype pattern.
    /// Represents an item that can be tracked in inventory with temporal and audit capabilities.
    /// This pattern can be reused for any entity requiring inventory-style management.
    /// </summary>
    public abstract class InventoryItem
    {
        /// <summary>
        /// Unique identifier for the inventory item
        /// </summary>
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Name of the inventory item
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Optional description providing details about the item
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Category for grouping similar items
        /// </summary>
        public string? Category { get; set; }

        /// <summary>
        /// When the item was added to inventory
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Who added the item (for audit purposes)
        /// </summary>
        public string? CreatedBy { get; set; }

        /// <summary>
        /// When the item was last modified
        /// </summary>
        public DateTime? LastModifiedAt { get; set; }

        /// <summary>
        /// Who last modified the item
        /// </summary>
        public string? LastModifiedBy { get; set; }

        /// <summary>
        /// Soft delete flag - items are marked as deleted but preserved in the system
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// When the item was marked as deleted
        /// </summary>
        public DateTime? DeletedAt { get; set; }

        /// <summary>
        /// Who deleted the item
        /// </summary>
        public string? DeletedBy { get; set; }

        /// <summary>
        /// Indicates if this is a standard/system-provided item
        /// </summary>
        public bool IsStandard { get; set; }

        /// <summary>
        /// Marks the item as deleted with audit information
        /// </summary>
        public virtual void MarkAsDeleted(string? deletedBy = null)
        {
            IsDeleted = true;
            DeletedAt = DateTime.UtcNow;
            DeletedBy = deletedBy;
        }

        /// <summary>
        /// Updates modification tracking
        /// </summary>
        public virtual void UpdateModificationTracking(string? modifiedBy = null)
        {
            LastModifiedAt = DateTime.UtcNow;
            LastModifiedBy = modifiedBy;
        }
    }
}
