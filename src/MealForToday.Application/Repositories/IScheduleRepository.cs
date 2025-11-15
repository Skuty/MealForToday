using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MealForToday.Application.Models;

namespace MealForToday.Application.Repositories
{
    public interface IScheduleRepository
    {
        Task<MealSchedule?> GetByIdAsync(Guid id);
        Task<List<MealSchedule>> GetAllAsync();
        Task AddAsync(MealSchedule schedule);
        Task UpdateAsync(MealSchedule schedule);
        Task DeleteAsync(Guid id);
    }
}
