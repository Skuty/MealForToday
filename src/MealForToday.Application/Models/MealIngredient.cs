using System;

namespace MealForToday.Application.Models
{
    /// <summary>
    /// Represents an ingredient used in a meal with its quantity.
    /// The Quantity property can be used as a multiplier of the ingredient's default amount.
    /// </summary>
    public class MealIngredient
    {
        public Guid IngredientId { get; set; }
        public Ingredient? Ingredient { get; set; }

        /// <summary>
        /// Quantity of the ingredient. Can be used as a multiplier of the default ingredient amount.
        /// Example: If ingredient default is 100g, Quantity=2 means 200g
        /// </summary>
        public decimal Quantity { get; set; }
        
        /// <summary>
        /// Unit of measurement (e.g., g, kg, ml, l)
        /// If not specified, uses the Ingredient's DefaultUnit
        /// </summary>
        public string? Unit { get; set; }
        
        /// <summary>
        /// Optional notes about this ingredient in the meal (e.g., "finely chopped", "optional")
        /// </summary>
        public string? Notes { get; set; }
    }
}
