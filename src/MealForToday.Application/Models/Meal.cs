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
        
        /// <summary>
        /// Author of the meal (User ID or username).
        /// Meals are available to all users, but we track who created them.
        /// </summary>
        public string? AuthorId { get; set; }
        
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime LastModifiedDate { get; set; } = DateTime.UtcNow;
        public List<MealIngredient> Ingredients { get; set; } = new List<MealIngredient>();
    }
}
