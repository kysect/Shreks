namespace Kysect.Shreks.Application.Abstractions.Exceptions;

public class EntityNotFoundException : NotFoundException
{
    public EntityNotFoundException(string? message) : base(message) { }
}