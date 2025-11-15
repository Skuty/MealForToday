using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MealForToday.Application.Models;

namespace MealForToday.Application.Services
{
    public interface IScheduleService
    {
        Task<MealSchedule> GenerateScheduleAsync(DateTime start, DateTime end, int mealsPerDay);
        Task<List<MealSchedule>> GetAllAsync();
        Task<MealSchedule?> GetByIdAsync(Guid id);
        Task SaveAsync(MealSchedule schedule);
    }
}
