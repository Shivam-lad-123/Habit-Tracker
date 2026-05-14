using HabitTracker.Models;

namespace HabitTracker.DTOs;

public sealed record UpdateHabitRequest(
    string Name,
    string? Description,
    HabitFrequency Frequency,
    int? TargetDays);