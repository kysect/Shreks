namespace Kysect.Shreks.Core.Models;

public readonly record struct StepperCollection<T>
{
    private readonly IReadOnlyList<T> _collection;
    private readonly int _index;

    public StepperCollection(IReadOnlyList<T> collection, int index)
    {
        _collection = collection;
        _index = index;
    }

    public T Current => _collection[_index];

    public bool AtEnd => _index == _collection.Count - 1;

    public StepperCollection<T> Next()
    {
        if (_index == _collection.Count - 1)
            throw new ArgumentOutOfRangeException(nameof(_index), "StepperCollection is at the end of the collection");

        return new StepperCollection<T>(_collection, _index + 1);
    }
}