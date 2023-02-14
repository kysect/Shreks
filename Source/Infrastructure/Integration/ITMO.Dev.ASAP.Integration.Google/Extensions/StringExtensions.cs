using System.Text.RegularExpressions;

namespace ITMO.Dev.ASAP.Integration.Google.Extensions;

internal static class StringExtensions
{
    private static readonly Regex HasCyrillicRegex = new Regex(@"\p{IsCyrillic}", RegexOptions.Compiled);

    public static bool HasCyrillic(this string s)
    {
        return HasCyrillicRegex.IsMatch(s);
    }
}