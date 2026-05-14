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
            Name = request.Name.Trim(),
            Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim(),
            Frequency = request.Frequency,
            TargetDays = request.Frequency == HabitFrequency.Weekly ? request.TargetDays : null,
            CreatedAt = DateTime.UtcNow
        };

        dbContext.Habits.Add(habit);
        await dbContext.SaveChangesAsync(cancellationToken);

        return ToResponse(habit);
    }

    public async Task<HabitResponse> UpdateAsync(int id, UpdateHabitRequest request, CancellationToken cancellationToken = default)
    {
        var habit = await dbContext.Habits.FirstOrDefaultAsync(habit => habit.Id == id, cancellationToken)
            ?? throw new KeyNotFoundException($"Habit {id} was not found.");

        habit.Name = request.Name.Trim();
        habit.Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim();
        habit.Frequency = request.Frequency;
        habit.TargetDays = request.Frequency == HabitFrequency.Weekly ? request.TargetDays : null;

        await dbContext.SaveChangesAsync(cancellationToken);

        return ToResponse(habit);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var habit = await dbContext.Habits.FirstOrDefaultAsync(habit => habit.Id == id, cancellationToken)
            ?? throw new KeyNotFoundException($"Habit {id} was not found.");

        dbContext.Habits.Remove(habit);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<HabitResponse> CompleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var habit = await dbContext.Habits.FirstOrDefaultAsync(habit => habit.Id == id, cancellationToken)
            ?? throw new KeyNotFoundException($"Habit {id} was not found.");

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        streakService.MarkCompleted(habit, today);

        await dbContext.SaveChangesAsync(cancellationToken);

        return ToResponse(habit);
    }

    public async Task<HabitResponse> ArchiveAsync(int id, CancellationToken cancellationToken = default)
    {
        var habit = await dbContext.Habits.FirstOrDefaultAsync(habit => habit.Id == id, cancellationToken)
            ?? throw new KeyNotFoundException($"Habit {id} was not found.");

        if (habit.IsArchived)
        {
            return ToResponse(habit);
        }

        habit.IsArchived = true;
        await dbContext.SaveChangesAsync(cancellationToken);

        return ToResponse(habit);
    }

    public async Task<HabitResponse> RestoreAsync(int id, CancellationToken cancellationToken = default)
    {
        var habit = await dbContext.Habits.FirstOrDefaultAsync(habit => habit.Id == id, cancellationToken)
            ?? throw new KeyNotFoundException($"Habit {id} was not found.");

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
            habit.CurrentStreak,
            habit.LongestStreak,
            habit.LastCompletedDate,
            habit.IsArchived,
            habit.CreatedAt);
    }

    private static System.Linq.Expressions.Expression<Func<Habit, HabitResponse>> ToResponseExpression()
    {
        return habit => new HabitResponse(
            habit.Id,
            habit.Name,
            habit.Description,
            habit.Frequency,
            habit.TargetDays,
            habit.CurrentStreak,
            habit.LongestStreak,
            habit.LastCompletedDate,
            habit.IsArchived,
            habit.CreatedAt);
    }
}