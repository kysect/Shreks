namespace Kysect.Shreks.Application.Common.Exceptions;

public class EntityNotFoundException : ShreksApplicationException
{
    public EntityNotFoundException(string? message) : base(message) { }
}