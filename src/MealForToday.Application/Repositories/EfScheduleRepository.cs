using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MealForToday.Application.Models;

namespace MealForToday.Application.Repositories
{
    public class EfScheduleRepository : IScheduleRepository
    {
        private readonly ApplicationDbContext _db;

        public EfScheduleRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task AddAsync(MealSchedule schedule)
        {
            _db.MealSchedules.Add(schedule);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var s = await _db.MealSchedules.FindAsync(id);
            if (s != null)
            {
                _db.MealSchedules.Remove(s);
                await _db.SaveChangesAsync();
            }
        }

        public async Task<List<MealSchedule>> GetAllAsync()
        {
            return await _db.MealSchedules.AsNoTracking().ToListAsync();
        }

        public async Task<MealSchedule?> GetByIdAsync(Guid id)
        {
            return await _db.MealSchedules.Include(x => x.ScheduleEntries).FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task UpdateAsync(MealSchedule schedule)
        {
            _db.MealSchedules.Update(schedule);
            await _db.SaveChangesAsync();
        }
    }
}
