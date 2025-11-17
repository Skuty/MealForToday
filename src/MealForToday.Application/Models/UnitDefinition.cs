using System;

namespace MealForToday.Application.Models
{
    /// <summary>
    /// Defines standard units of measurement with their base conversion factors.
    /// Used to calculate ingredient quantities as multiples of default amounts.
    /// Implements the Inventory archetype pattern for reusability.
    /// </summary>
    public class UnitDefinition : InventoryItem
    {
        /// <summary>
        /// Unit code (e.g., "kg", "g", "l", "ml")
        /// </summary>
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// Base amount multiplier. This represents how many of this unit make up the base measure.
        /// Example: 
        /// - Kilogram: 1 (base unit for weight)
        /// - Gram: 1000 (1000 grams = 1 kilogram)
        /// - Litre: 1 (base unit for volume)
        /// - Milliliter: 1000 (1000 milliliters = 1 litre)
        /// </summary>
        public decimal BaseAmount { get; set; }

        /// <summary>
        /// Type of measurement (weight, volume, count, etc.)
        /// Stored in Category property from InventoryItem base class, but exposed here for clarity.
        /// </summary>
        public string? MeasurementType
        {
            get => Category;
            set => Category = value;
        }
    }
}
