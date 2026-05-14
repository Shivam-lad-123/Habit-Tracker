using HabitTracker.DTOs;

namespace HabitTracker.Services;

public interface IHabitService
{
    Task<IReadOnlyList<HabitResponse>> GetAllAsync(bool includeArchived, CancellationToken cancellationToken = default);

    Task<HabitResponse> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<HabitResponse> CreateAsync(CreateHabitRequest request, CancellationToken cancellationToken = default);

    Task<HabitResponse> UpdateAsync(int id, UpdateHabitRequest request, CancellationToken cancellationToken = default);

    Task DeleteAsync(int id, CancellationToken cancellationToken = default);

    Task<HabitResponse> CompleteAsync(int id, CancellationToken cancellationToken = default);

    Task<HabitResponse> ArchiveAsync(int id, CancellationToken cancellationToken = default);

    Task<HabitResponse> RestoreAsync(int id, CancellationToken cancellationToken = default);

    Task<HabitSummaryResponse> GetSummaryAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<HabitResponse>> GetDueTodayAsync(CancellationToken cancellationToken = default);
}