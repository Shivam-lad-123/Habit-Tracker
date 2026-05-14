using HabitTracker.Models;
using Microsoft.EntityFrameworkCore;

namespace HabitTracker.Data;

public static class HabitSeeder
{
    public static async Task SeedAsync(HabitTrackerDbContext dbContext, CancellationToken cancellationToken = default)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var yesterday = today.AddDays(-1);
        var lastWeek = today.AddDays(-7);
        var startOfThisWeek = GetStartOfWeek(today);

        await EnsureHabitAsync(dbContext, cancellationToken, new Habit
        {
            Name = "Morning Run",
            Description = "Daily cardio habit that is due today.",
            Frequency = HabitFrequency.Daily,
            CurrentStreak = 2,
            LongestStreak = 5,
            LastCompletedDate = yesterday,
            IsArchived = false,
            CreatedAt = DateTime.UtcNow.AddDays(-10)
        });

        await EnsureHabitAsync(dbContext, cancellationToken, new Habit
        {
            Name = "Read 20 Pages",
            Description = "Already completed today so complete should return conflict.",
            Frequency = HabitFrequency.Daily,
            CurrentStreak = 3,
            LongestStreak = 4,
            LastCompletedDate = today,
            IsArchived = false,
            CreatedAt = DateTime.UtcNow.AddDays(-12)
        });

        await EnsureHabitAsync(dbContext, cancellationToken, new Habit
        {
            Name = "Weekly Planning",
            Description = "Weekly habit that has not been completed this week.",
            Frequency = HabitFrequency.Weekly,
            TargetDays = 3,
            CurrentStreak = 1,
            LongestStreak = 2,
            LastCompletedDate = lastWeek,
            IsArchived = false,
            CreatedAt = DateTime.UtcNow.AddDays(-20)
        });

        await EnsureHabitAsync(dbContext, cancellationToken, new Habit
        {
            Name = "Meal Prep",
            Description = "Weekly habit already completed this week so it should not appear due today.",
            Frequency = HabitFrequency.Weekly,
            TargetDays = 2,
            CurrentStreak = 4,
            LongestStreak = 6,
            LastCompletedDate = startOfThisWeek,
            IsArchived = false,
            CreatedAt = DateTime.UtcNow.AddDays(-15)
        });

        await EnsureHabitAsync(dbContext, cancellationToken, new Habit
        {
            Name = "Archived Stretching",
            Description = "Archived habit for testing the includeArchived filter.",
            Frequency = HabitFrequency.Daily,
            CurrentStreak = 0,
            LongestStreak = 1,
            LastCompletedDate = null,
            IsArchived = true,
            CreatedAt = DateTime.UtcNow.AddDays(-30)
        });

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private static async Task EnsureHabitAsync(HabitTrackerDbContext dbContext, CancellationToken cancellationToken, Habit habit)
    {
        var existingHabit = await dbContext.Habits.FirstOrDefaultAsync(existing => existing.Name == habit.Name, cancellationToken);
        if (existingHabit is null)
        {
            dbContext.Habits.Add(habit);
            return;
        }

        existingHabit.Description = habit.Description;
        existingHabit.Frequency = habit.Frequency;
        existingHabit.TargetDays = habit.TargetDays;
        existingHabit.CurrentStreak = habit.CurrentStreak;
        existingHabit.LongestStreak = habit.LongestStreak;
        existingHabit.LastCompletedDate = habit.LastCompletedDate;
        existingHabit.IsArchived = habit.IsArchived;
        existingHabit.CreatedAt = habit.CreatedAt;
    }

    private static DateOnly GetStartOfWeek(DateOnly date)
    {
        var dayOffset = ((int)date.DayOfWeek + 6) % 7;
        return date.AddDays(-dayOffset);
    }
}
