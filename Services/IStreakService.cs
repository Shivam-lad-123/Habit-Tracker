using HabitTracker.Models;

namespace HabitTracker.Services;

public interface IStreakService
{
    void MarkCompleted(Habit habit, DateOnly completedDate);

    bool IsDueToday(Habit habit, DateOnly today);
}