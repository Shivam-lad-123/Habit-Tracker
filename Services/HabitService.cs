using HabitTracker.Data;
using HabitTracker.DTOs;
using HabitTracker.Models;
using Microsoft.EntityFrameworkCore;

namespace HabitTracker.Services;

public sealed class HabitService : IHabitService
{
    private readonly HabitTrackerDbContext dbContext;
    private readonly IStreakService streakService;

    public HabitService(HabitTrackerDbContext dbContext, IStreakService streakService)
    {
        this.dbContext = dbContext;
        this.streakService = streakService;
    }

    public async Task<IReadOnlyList<HabitResponse>> GetAllAsync(bool includeArchived, CancellationToken cancellationToken = default)
    {
        var query = dbContext.Habits.AsNoTracking();

        if (!includeArchived)
        {
            query = query.Where(habit => !habit.IsArchived);
        }

        return await query
            .OrderBy(habit => habit.Name)
            .Select(ToResponseExpression())
            .ToListAsync(cancellationToken);
    }

    public async Task<HabitResponse> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var habit = await dbContext.Habits.AsNoTracking().FirstOrDefaultAsync(habit => habit.Id == id, cancellationToken)
            ?? throw new KeyNotFoundException($"Habit {id} was not found.");

        return ToResponse(habit);
    }

    public async Task<HabitResponse> CreateAsync(CreateHabitRequest request, CancellationToken cancellationToken = default)
    {
        var habit = new Habit
        {
            CreatedAt = DateTime.UtcNow
        };

        ApplyRequest(habit, request.Name, request.Description, request.Frequency, request.TargetDays);

        dbContext.Habits.Add(habit);
        await dbContext.SaveChangesAsync(cancellationToken);

        return ToResponse(habit);
    }

    public async Task<HabitResponse> UpdateAsync(int id, UpdateHabitRequest request, CancellationToken cancellationToken = default)
    {
        var habit = await GetTrackedHabitAsync(id, cancellationToken);

        ApplyRequest(habit, request.Name, request.Description, request.Frequency, request.TargetDays);

        await dbContext.SaveChangesAsync(cancellationToken);

        return ToResponse(habit);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var habit = await GetTrackedHabitAsync(id, cancellationToken);

        dbContext.Habits.Remove(habit);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<HabitResponse> CompleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var habit = await GetTrackedHabitAsync(id, cancellationToken);

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        streakService.MarkCompleted(habit, today);

        await dbContext.SaveChangesAsync(cancellationToken);

        return ToResponse(habit);
    }

    public async Task<HabitResponse> ArchiveAsync(int id, CancellationToken cancellationToken = default)
    {
        var habit = await GetTrackedHabitAsync(id, cancellationToken);

        if (habit.IsArchived)
        {
            return ToResponse(habit);
        }

        habit.IsArchived = true;
        await dbContext.SaveChangesAsync(cancellationToken);

        return ToResponse(habit);
    }

    public async Task<HabitResponse> UnarchiveAsync(int id, CancellationToken cancellationToken = default)
    {
        var habit = await GetTrackedHabitAsync(id, cancellationToken);

        if (!habit.IsArchived)
        {
            return ToResponse(habit);
        }

        habit.IsArchived = false;
        await dbContext.SaveChangesAsync(cancellationToken);

        return ToResponse(habit);
    }

    public async Task<HabitSummaryResponse> GetSummaryAsync(CancellationToken cancellationToken = default)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        var habits = await dbContext.Habits.AsNoTracking().ToListAsync(cancellationToken);

        return new HabitSummaryResponse(
            habits.Count,
            habits.Count(habit => habit.LastCompletedDate == today),
            habits.Count == 0 ? 0 : habits.Max(habit => habit.LongestStreak));
    }

    public async Task<IReadOnlyList<HabitResponse>> GetDueTodayAsync(CancellationToken cancellationToken = default)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        var habits = await dbContext.Habits
            .AsNoTracking()
            .Where(habit => !habit.IsArchived)
            .ToListAsync(cancellationToken);

        return habits
            .Where(habit => streakService.IsDueToday(habit, today))
            .Select(ToResponse)
            .ToList();
    }

    private static HabitResponse ToResponse(Habit habit)
    {
        return new HabitResponse(
            habit.Id,
            habit.Name,
            habit.Description,
            habit.Frequency,
            habit.TargetDays,
            habit.ColorHex,
            habit.CurrentStreak,
            habit.LongestStreak,
            habit.LastCompletedDate,
            habit.IsArchived,
            habit.CreatedAt);
    }

    private static void ApplyRequest(Habit habit, string name, string? description, HabitFrequency frequency, int? targetDays)
    {
        habit.Name = name.Trim();
        habit.Description = NormalizeDescription(description);
        habit.Frequency = frequency;
        habit.TargetDays = frequency == HabitFrequency.Weekly ? targetDays : null;
    }

    private async Task<Habit> GetTrackedHabitAsync(int id, CancellationToken cancellationToken)
    {
        return await dbContext.Habits.FirstOrDefaultAsync(habit => habit.Id == id, cancellationToken)
            ?? throw new KeyNotFoundException($"Habit {id} was not found.");
    }

    private static string? NormalizeDescription(string? description)
    {
        return string.IsNullOrWhiteSpace(description) ? null : description.Trim();
    }

    private static System.Linq.Expressions.Expression<Func<Habit, HabitResponse>> ToResponseExpression()
    {
        return habit => new HabitResponse(
            habit.Id,
            habit.Name,
            habit.Description,
            habit.Frequency,
            habit.TargetDays,
            habit.ColorHex,
            habit.CurrentStreak,
            habit.LongestStreak,
            habit.LastCompletedDate,
            habit.IsArchived,
            habit.CreatedAt);
    }
}