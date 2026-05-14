using HabitTracker.Models;

namespace HabitTracker.DTOs;

public sealed record CreateHabitRequest(
    string Name,
    string? Description,
    HabitFrequency Frequency,
    int? TargetDays);