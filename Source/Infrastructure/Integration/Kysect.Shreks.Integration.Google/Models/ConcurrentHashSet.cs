namespace Kysect.Shreks.Integration.Google.Models;

public class ConcurrentHashSet<T>
{
    private readonly object _lock = new();

    private readonly HashSet<T> _hastSet = new();

    public void Add(T value)
    {
        lock (_lock)
        {
            _hastSet.Add(value);
        }
    }

    public IReadOnlyCollection<T> GetAndClearValues()
    {
        lock (_lock)
        {
            var values = _hastSet.ToArray();
            _hastSet.Clear();
            return values;
        }
    }
}