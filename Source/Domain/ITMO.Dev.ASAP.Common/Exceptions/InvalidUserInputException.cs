using ITMO.Dev.ASAP.Common.Resources;

namespace ITMO.Dev.ASAP.Common.Exceptions;

public class InvalidUserInputException : DomainException
{
    public InvalidUserInputException(string? message)
        : base(message) { }

    public static InvalidUserInputException FailedToParseUserCommand()
    {
        return new InvalidUserInputException(UserMessages.FailedToParseUserCommand);
    }
}