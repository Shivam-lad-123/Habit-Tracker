using HabitTracker.Models;

namespace HabitTracker.DTOs;

public sealed record HabitResponse(
    int Id,
    string Name,
    string? Description,
    HabitFrequency Frequency,
    int? TargetDays,
    string? ColorHex,
    int CurrentStreak,
    int LongestStreak,
    DateOnly? LastCompletedDate,
    bool IsArchived,
    DateTime CreatedAt);