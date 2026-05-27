using HabitTracker.Models;
using Microsoft.EntityFrameworkCore;

namespace HabitTracker.Data;

public sealed class HabitTrackerDbContext : DbContext
{
    public HabitTrackerDbContext(DbContextOptions<HabitTrackerDbContext> options)
        : base(options)
    {
    }

    public DbSet<Habit> Habits => Set<Habit>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Habit>(entity =>
        {
            entity.HasKey(habit => habit.Id);

            entity.Property(habit => habit.Name)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(habit => habit.Description)
                .HasMaxLength(500);

            entity.Property(habit => habit.ColorHex)
                .HasMaxLength(7);

            entity.Property(habit => habit.Frequency)
                .HasConversion<int>()
                .IsRequired();

            entity.Property(habit => habit.CurrentStreak)
                .HasDefaultValue(0);

            entity.Property(habit => habit.LongestStreak)
                .HasDefaultValue(0);

            entity.Property(habit => habit.IsArchived)
                .HasDefaultValue(false);

            entity.Property(habit => habit.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");
        });
    }
}