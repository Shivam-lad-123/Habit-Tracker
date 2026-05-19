using FluentValidation;
using HabitTracker.DTOs;
using HabitTracker.Models;

namespace HabitTracker.Validators;

public sealed class CreateHabitRequestValidator : AbstractValidator<CreateHabitRequest>
{
    public CreateHabitRequestValidator()
    {
        RuleFor(request => request.Name)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(request => request.Frequency)
            .NotEmpty()
            .WithMessage("Frequency is required. Valid values are: Daily, Weekly")
            .IsInEnum()
            .WithMessage("Frequency must be a valid value. Valid values are: Daily, Weekly");

        RuleFor(request => request.TargetDays)
            .Custom((targetDays, context) =>
            {
                var request = context.InstanceToValidate;

                if (request.Frequency == HabitFrequency.Weekly)
                {
                    if (!targetDays.HasValue)
                    {
                        context.AddFailure(nameof(CreateHabitRequest.TargetDays), "TargetDays is required for weekly habits.");
                        return;
                    }

                    if (targetDays is < 1 or > 7)
                    {
                        context.AddFailure(nameof(CreateHabitRequest.TargetDays), "TargetDays must be between 1 and 7.");
                    }
                }
            });
    }
}