using FluentValidation;
using HabitTracker.Data;
using HabitTracker.Endpoints;
using HabitTracker.Services;
using HabitTracker.Validators;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

builder.Services.AddDbContext<HabitTrackerDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")
        ?? "Server=.;Database=HabitTracker;Trusted_Connection=True;TrustServerCertificate=True;"));

builder.Services.AddScoped<IHabitService, HabitService>();
builder.Services.AddScoped<IStreakService, StreakService>();

builder.Services.AddValidatorsFromAssemblyContaining<CreateHabitRequestValidator>();

var app = builder.Build();

app.UseExceptionHandler();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/openapi/v1.json", "Habit Tracker API v1");
    options.RoutePrefix = "swagger";
});

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<HabitTrackerDbContext>();
    await dbContext.Database.MigrateAsync();
    await HabitSeeder.SeedAsync(dbContext);
}

app.MapHabitEndpoints();

app.MapOpenApi();
app.MapScalarApiReference(options =>
{
    options.Title = "Habit Tracker API";
    options.Theme = ScalarTheme.BluePlanet;
});

app.Run();

public sealed class GlobalExceptionHandler : IExceptionHandler
{
    private readonly IProblemDetailsService problemDetailsService;

    public GlobalExceptionHandler(IProblemDetailsService problemDetailsService)
    {
        this.problemDetailsService = problemDetailsService;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var statusCode = exception switch
        {
            KeyNotFoundException => StatusCodes.Status404NotFound,
            InvalidOperationException => StatusCodes.Status409Conflict,
            _ => StatusCodes.Status500InternalServerError
        };

        httpContext.Response.StatusCode = statusCode;

        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails = new ProblemDetails
            {
                Status = statusCode,
                Title = statusCode switch
                {
                    StatusCodes.Status404NotFound => "Not Found",
                    StatusCodes.Status409Conflict => "Conflict",
                    _ => "An unexpected error occurred"
                },
                Detail = exception.Message,
                Instance = httpContext.Request.Path
            }
        });
    }
}