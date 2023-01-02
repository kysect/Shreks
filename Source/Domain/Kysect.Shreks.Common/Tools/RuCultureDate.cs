using Kysect.Shreks.Common.Exceptions;
using System.Globalization;

namespace Kysect.Shreks.Common.Tools;

public class RuCultureDate
{
    public static DateOnly Parse(string? value)
    {
        if (!DateOnly.TryParse(value, CultureInfo.GetCultureInfo("ru-Ru"), DateTimeStyles.None, out DateOnly date))
            throw new InvalidUserInputException($"Cannot parse input date ({value} as date. Ensure that you use correct format.");

        return date;
    }
}