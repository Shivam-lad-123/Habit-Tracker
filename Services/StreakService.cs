using HabitTracker.Models;

namespace HabitTracker.Services;

public sealed class StreakService : IStreakService
{
    public void MarkCompleted(Habit habit, DateOnly completedDate)
    {
        if (habit.LastCompletedDate == completedDate)
        {
            throw new InvalidOperationException("This habit has already been completed today.");
        }

        if (habit.LastCompletedDate == completedDate.AddDays(-1))
        {
            habit.CurrentStreak++;
        }
        else
        {
            habit.CurrentStreak = 1;
        }

        if (habit.CurrentStreak > habit.LongestStreak)
        {
            habit.LongestStreak = habit.CurrentStreak;
        }

        habit.LastCompletedDate = completedDate;
    }

    public bool IsDueToday(Habit habit, DateOnly today)
    {
        return habit.Frequency switch
        {
            HabitFrequency.Daily => habit.LastCompletedDate != today,
            HabitFrequency.Weekly => !habit.LastCompletedDate.HasValue || habit.LastCompletedDate.Value < StartOfWeek(today),
            _ => false
        };
    }

    private static DateOnly StartOfWeek(DateOnly date)
    {
        var dayOffset = ((int)date.DayOfWeek + 6) % 7;
        return date.AddDays(-dayOffset);
    }
}