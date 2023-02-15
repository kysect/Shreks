namespace ITMO.Dev.ASAP.Common.Exceptions;

public class RegistrationFailedException : DomainException
{
    public RegistrationFailedException(string? message) : base(message) { }
}