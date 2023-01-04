namespace Kysect.Shreks.Application.Extensions;

public static class EnumerableExtensions
{
    public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T?> enumerable)
        where T : class
    {
        return enumerable.Where(x => x is not null).Select(x => x!);
    }

    public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T?> enumerable)
        where T : struct
    {
        return enumerable.Where(x => x is not null).Select(x => x!.Value);
    }
}