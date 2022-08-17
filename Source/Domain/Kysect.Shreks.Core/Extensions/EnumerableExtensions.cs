using System.Runtime.CompilerServices;

namespace Kysect.Shreks.Core.Extensions;

internal static class EnumerableExtensions
{
    // Async enumerable generator with `yield return` must be an `async` method.
#pragma warning disable CS1998
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