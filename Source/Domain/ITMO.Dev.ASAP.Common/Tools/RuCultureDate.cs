using ITMO.Dev.ASAP.Common.Exceptions;
using System.Globalization;

namespace ITMO.Dev.ASAP.Common.Tools;

public class RuCultureDate
{
    public static DateOnly Parse(string? value)
    {
        return !DateOnly.TryParse(value, CultureInfo.GetCultureInfo("ru-Ru"), DateTimeStyles.None, out DateOnly date)
            ? throw new InvalidUserInputException(
                $"Cannot parse input date ({value} as date. Ensure that you use correct format.")
            : date;
    }
}