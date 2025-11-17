using System;

namespace MealForToday.Application.Models
{
    /// <summary>
    /// Ingredient entity implementing the Inventory archetype pattern.
    /// Represents a food ingredient that can be tracked and managed in the system's inventory.
    /// </summary>
    public class Ingredient : InventoryItem
    {
        /// <summary>
        /// Default unit of measurement for this ingredient (e.g., g, ml, piece)
        /// </summary>
        public string? DefaultUnit { get; set; }

        /// <summary>
        /// Nutritional information: calories per 100g of the ingredient
        /// </summary>
        public decimal? CaloriesPer100g { get; set; }
    }
}
