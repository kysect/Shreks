namespace Kysect.Shreks.Application.Abstractions.Exceptions;

public class UnauthorizedException : ShreksApplicationException
{
    public UnauthorizedException(string? message) : base(message) { }
}