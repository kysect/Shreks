namespace Kysect.Shreks.WebUI.AdminPanel.Tools;

public static class EqualityComparerFactory
{
    public static IEqualityComparer<T> Create<T>(Func<T, T, bool> comparer)
    {
        return new GenericEqualityComparer<T>(comparer);
    }

    private class GenericEqualityComparer<T> : IEqualityComparer<T>
    {
        private readonly Func<T, T, bool> _comparer;

        public GenericEqualityComparer(Func<T, T, bool> comparer)
        {
            _comparer = comparer;
        }

        public bool Equals(T? x, T? y)
        {
            return (x, y) switch
            {
                (null, null) => true,
                (null, not null) or (not null, null) => false,
                _ => _comparer.Invoke(x, y),
            };
        }

        public int GetHashCode(T obj)
        {
            return obj?.GetHashCode() ?? 0;
        }
    }
}