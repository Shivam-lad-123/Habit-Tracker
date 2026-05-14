using FluentValidation;
using HabitTracker.DTOs;
using HabitTracker.Services;

namespace HabitTracker.Endpoints;

public static class HabitEndpoints
{
    public static IEndpointRouteBuilder MapHabitEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/habits");

        group.MapGet(string.Empty, async (bool? includeArchived, IHabitService habitService, CancellationToken cancellationToken) =>
            Results.Ok(await habitService.GetAllAsync(includeArchived ?? false, cancellationToken)));

        group.MapGet("summary", async (IHabitService habitService, CancellationToken cancellationToken) =>
            Results.Ok(await habitService.GetSummaryAsync(cancellationToken)));

        group.MapGet("due-today", async (IHabitService habitService, CancellationToken cancellationToken) =>
            Results.Ok(await habitService.GetDueTodayAsync(cancellationToken)));

        group.MapGet("{id:int}", async (int id, IHabitService habitService, CancellationToken cancellationToken) =>
            Results.Ok(await habitService.GetByIdAsync(id, cancellationToken)));

        group.MapPost(string.Empty, async (CreateHabitRequest request, IValidator<CreateHabitRequest> validator, IHabitService habitService, CancellationToken cancellationToken) =>
        {
            var validationResult = await validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                return Results.ValidationProblem(validationResult.ToDictionary());
            }

            var habit = await habitService.CreateAsync(request, cancellationToken);
            return Results.Created($"/habits/{habit.Id}", habit);
        });

        group.MapPut("{id:int}", async (int id, UpdateHabitRequest request, IValidator<UpdateHabitRequest> validator, IHabitService habitService, CancellationToken cancellationToken) =>
        {
            var validationResult = await validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                return Results.ValidationProblem(validationResult.ToDictionary());
            }

            return Results.Ok(await habitService.UpdateAsync(id, request, cancellationToken));
        });

        group.MapDelete("{id:int}", async (int id, IHabitService habitService, CancellationToken cancellationToken) =>
        {
            await habitService.DeleteAsync(id, cancellationToken);
            return Results.NoContent();
        });

        group.MapPatch("{id:int}/complete", async (int id, IHabitService habitService, CancellationToken cancellationToken) =>
            Results.Ok(await habitService.CompleteAsync(id, cancellationToken)));

        group.MapPatch("{id:int}/archive", async (int id, IHabitService habitService, CancellationToken cancellationToken) =>
            Results.Ok(await habitService.ArchiveAsync(id, cancellationToken)));

        return endpoints;
    }
}