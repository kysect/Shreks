namespace Kysect.Shreks.Common.Exceptions;

public class RegistrationFailedException : ShreksDomainException
{
    public RegistrationFailedException(string? message) : base(message) { }
}