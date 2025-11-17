using Microsoft.EntityFrameworkCore;
using MealForToday.Application.Models;

namespace MealForToday.Application
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Meal> Meals { get; set; }
        public DbSet<Ingredient> Ingredients { get; set; }
        public DbSet<MealSchedule> MealSchedules { get; set; }
        public DbSet<UnitDefinition> UnitDefinitions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Meal>(b =>
            {
                b.HasKey(x => x.Id);
                b.Property(x => x.Name).IsRequired();
                b.OwnsMany(x => x.Ingredients, mb =>
                {
                    mb.WithOwner().HasForeignKey("MealId");
                    mb.Property<Guid>("Id");
                });
            });

            modelBuilder.Entity<Ingredient>(b =>
            {
                b.HasKey(x => x.Id);
                b.Property(x => x.Name).IsRequired();
                b.HasQueryFilter(x => !x.IsDeleted);
            });

            modelBuilder.Entity<MealSchedule>(b =>
            {
                b.HasKey(x => x.Id);
                b.OwnsMany(x => x.ScheduleEntries, sb =>
                {
                    sb.WithOwner().HasForeignKey("MealScheduleId");
                    sb.Property<Guid>("Id");
                });
            });

            modelBuilder.Entity<UnitDefinition>(b =>
            {
                b.HasKey(x => x.Id);
                b.Property(x => x.Code).IsRequired();
                b.Property(x => x.Name).IsRequired();
                b.Property(x => x.BaseAmount).IsRequired();
                b.HasIndex(x => x.Code).IsUnique();
            });
        }
    }
}
