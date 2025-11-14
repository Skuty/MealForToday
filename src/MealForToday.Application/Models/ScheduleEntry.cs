using System;

namespace MealForToday.Application.Models
{
    public enum MealType
    {
        Breakfast,
        Lunch,
        Dinner,
        Snack
    }

    public class ScheduleEntry
    {
        public Guid EntryId { get; set; } = Guid.NewGuid();
        public Guid MealId { get; set; }
        public Meal? Meal { get; set; }
        public DateTime Date { get; set; }
        public MealType MealType { get; set; }
        public string? Notes { get; set; }
    }
}
