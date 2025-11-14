using System;
using System.Collections.Generic;

namespace MealForToday.Application.Models
{
    public class MealSchedule
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid? UserId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public List<ScheduleEntry> ScheduleEntries { get; set; } = new List<ScheduleEntry>();
    }
}
