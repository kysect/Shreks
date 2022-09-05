using System.Text.RegularExpressions;

namespace Kysect.Shreks.Integration.Google.Extensions;

internal static class StringExtensions
{
    private static readonly Regex HasCyrillicRegex = new(@"\p{IsCyrillic}");

    public static bool HasCyrillic(this string s)
        => HasCyrillicRegex.IsMatch(s);
}