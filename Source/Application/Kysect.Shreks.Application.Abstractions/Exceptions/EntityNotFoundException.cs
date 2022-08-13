namespace Kysect.Shreks.Application.Abstractions.Exceptions;

public class EntityNotFoundException : ShreksApplicationException
{
    public EntityNotFoundException(string? message) : base(message) { }
}