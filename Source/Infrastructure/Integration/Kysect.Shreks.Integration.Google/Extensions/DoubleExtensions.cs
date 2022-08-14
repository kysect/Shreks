using System.Globalization;

namespace Kysect.Shreks.Integration.Google.Extensions;

internal static class DoubleExtensions
{
    public static string ToSheetPoints(this double d, CultureInfo cultureInfo)
        => Math.Round(d, 2).ToString(cultureInfo);
}