namespace Kysect.Shreks.Integration.Google.Models;

public class ConcurrentHashSet<T>
{
    private readonly object _lock = new();

    private readonly HashSet<T> _hashSet = new();

    public void Add(T value)
    {
        lock (_lock)
        {
            _hashSet.Add(value);
        }
    }

    public IReadOnlyCollection<T> GetAndClearValues()
    {
        lock (_lock)
        {
            T[] values = _hashSet.ToArray();
            _hashSet.Clear();
            return values;
        }
    }
}