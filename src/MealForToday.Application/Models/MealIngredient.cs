using System;

namespace MealForToday.Application.Models
{
    public class MealIngredient
    {
        public Guid IngredientId { get; set; }
        public Ingredient? Ingredient { get; set; }

        public decimal Quantity { get; set; }
        public string? Unit { get; set; }
        public string? Notes { get; set; }
    }
}
