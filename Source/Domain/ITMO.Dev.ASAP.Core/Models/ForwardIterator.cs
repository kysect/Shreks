namespace ITMO.Dev.ASAP.Core.Models;

public readonly record struct ForwardIterator<T>
{
    private readonly IReadOnlyList<T> _collection;
    private readonly int _index;

    public ForwardIterator(IReadOnlyList<T> collection, int index)
    {
        _collection = collection;
        _index = index;
    }

    public T Current => _collection[_index];

    public bool IsAtEnd => _index == _collection.Count - 1;

    public ForwardIterator<T> Next()
    {
        if (_index == _collection.Count - 1)
            throw new ArgumentOutOfRangeException(nameof(_index), @"StepperCollection is at the end of the collection");

        return new ForwardIterator<T>(_collection, _index + 1);
    }
}