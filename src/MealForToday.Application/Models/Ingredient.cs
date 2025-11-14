using System;

namespace MealForToday.Application.Models
{
    public class Ingredient
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
        public string? Category { get; set; }
        public string? DefaultUnit { get; set; }
        public decimal? CaloriesPer100g { get; set; }
        public bool IsStandard { get; set; }
    }
}
