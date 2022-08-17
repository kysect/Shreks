using System.Runtime.CompilerServices;

namespace Kysect.Shreks.Core.Extensions;

// Async enumerable generator with `yield return` must be an `async` method.
#pragma warning disable CS1998

internal static class EnumerableExtensions
{
    public static async IAsyncEnumerable<T> AsAsyncEnumerable<T>(
        this IEnumerable<T> enumerable,
        [EnumeratorCancellation] CancellationToken _ = default)
    {
        foreach (var value in enumerable)
        {
            yield return value;
        }
    }
}