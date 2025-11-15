using System;
using System.Collections.Generic;

namespace MealForToday.Application.Models
{
    public class Meal
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? CuisineType { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime LastModifiedDate { get; set; } = DateTime.UtcNow;
        public List<MealIngredient> Ingredients { get; set; } = new List<MealIngredient>();
    }
}
