namespace Kysect.Shreks.Common.Exceptions;

public class EntityNotFoundException : NotFoundException
{

    public static EntityNotFoundException For<T>(Guid id)
    {
        return new EntityNotFoundException($"Entity of type {typeof(T).Name} with id {id} not found");
    }

    public EntityNotFoundException(string? message) : base(message) { }
}