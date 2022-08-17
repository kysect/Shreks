using System.Runtime.CompilerServices;

namespace Kysect.Shreks.Core.Extensions;

internal static class EnumerableExtensions
{
    public static async IAsyncEnumerable<T> AsAsyncEnumerable<T>(
        this IEnumerable<T> enumerable,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        foreach (var value in enumerable)
        {
            yield return value;
        }
    }
}