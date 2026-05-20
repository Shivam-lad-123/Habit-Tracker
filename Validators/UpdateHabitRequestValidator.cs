using FluentValidation;
using HabitTracker.DTOs;
using HabitTracker.Models;

namespace HabitTracker.Validators;

public sealed class UpdateHabitRequestValidator : AbstractValidator<UpdateHabitRequest>
{
    public UpdateHabitRequestValidator()
    {
        RuleFor(request => request.Name)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(request => request.Frequency)
            .IsInEnum()
            .WithMessage($"Frequency must be one of the following values: {ValidatorExtensions.GetValidFrequenciesMessage()}.");

        RuleFor(request => request.TargetDays)
            .Custom((targetDays, context) =>
            {
                var request = context.InstanceToValidate;

                if (request.Frequency == HabitFrequency.Weekly)
                {
                    if (!targetDays.HasValue)
                    {
                        context.AddFailure(nameof(UpdateHabitRequest.TargetDays), "TargetDays is required for weekly habits.");
                        return;
                    }

                    if (targetDays is < 1 or > 7)
                    {
                        context.AddFailure(nameof(UpdateHabitRequest.TargetDays), "TargetDays must be between 1 and 7.");
                    }
                }
            });
    }
}