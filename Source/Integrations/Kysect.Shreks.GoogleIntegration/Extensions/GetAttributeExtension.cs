using System.Collections.Concurrent;
using System.Reflection;
using Kysect.Shreks.GoogleIntegration.Attributes;
using Kysect.Shreks.GoogleIntegration.Sheets;

namespace Kysect.Shreks.GoogleIntegration.Extensions;

public static class GetAttributeExtensions
{
    private static readonly ConcurrentDictionary<Type, GoogleSheetAttribute> Attributes = new();

    public static GoogleSheetAttribute GetGoogleSheetAttribute(this Type type)
        => Attributes.GetOrAdd(
            type,
            t => t.GetCustomAttribute<GoogleSheetAttribute>()!);

    public static GoogleSheetAttribute GetGoogleSheetAttribute<T>(this T sheet)
        where T : ISheet, new()
        => sheet.GetType().GetGoogleSheetAttribute();
}