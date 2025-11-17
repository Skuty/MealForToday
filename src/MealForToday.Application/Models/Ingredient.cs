using System;

namespace MealForToday.Application.Models
{
    public class Ingredient
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Category { get; set; }
        public string? DefaultUnit { get; set; }
        public decimal? CaloriesPer100g { get; set; }
        public bool IsStandard { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}
