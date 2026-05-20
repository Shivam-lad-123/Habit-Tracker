using HabitTracker.Models;

namespace HabitTracker.Validators;

public static class ValidatorExtensions
{
    private static readonly string ValidFrequenciesMessage = GetValidFrequenciesInternal();

    public static string GetValidFrequenciesMessage()
    {
        return ValidFrequenciesMessage;
    }

    private static string GetValidFrequenciesInternal()
    {
        var values = Enum.GetNames(typeof(HabitFrequency));
        return string.Join(", ", values);
    }
}
