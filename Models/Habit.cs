namespace HabitTracker.Models;

public sealed class Habit
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public HabitFrequency Frequency { get; set; }

    public int? TargetDays { get; set; }

    public string? ColorHex { get; set; }

    public int CurrentStreak { get; set; }

    public int LongestStreak { get; set; }

    public DateOnly? LastCompletedDate { get; set; }

    public bool IsArchived { get; set; }

    public DateTime CreatedAt { get; set; }
}