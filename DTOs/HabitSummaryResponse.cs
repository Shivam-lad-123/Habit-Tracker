namespace HabitTracker.DTOs;

public sealed record HabitSummaryResponse(
    int TotalHabits,
    int CompletedToday,
    int CurrentLongestStreak);