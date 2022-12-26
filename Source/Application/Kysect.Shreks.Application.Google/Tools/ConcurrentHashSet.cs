namespace Kysect.Shreks.Application.Google.Tools;

public class ConcurrentHashSet<T>
{
    private readonly object _lock = new object();
    private readonly HashSet<T> _hashSet = new HashSet<T>();

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