namespace Kysect.Shreks.Common.Exceptions;

public class UnauthorizedException : ShreksDomainException
{
    public UnauthorizedException(string? message) : base(message) { }
}