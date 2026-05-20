using HabitTracker.Models;

namespace HabitTracker.Validators;

public static class ValidatorExtensions
{
    public static string GetValidFrequenciesMessage()
    {
        var values = Enum.GetNames(typeof(HabitFrequency));
        return string.Join(", ", values);
    }
}
