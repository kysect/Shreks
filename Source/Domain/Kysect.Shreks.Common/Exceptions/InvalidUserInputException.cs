using Kysect.Shreks.Common.Resources;

namespace Kysect.Shreks.Common.Exceptions;

public class InvalidUserInputException : ShreksDomainException
{
    public InvalidUserInputException(string? message)
        : base(message) { }

    public static InvalidUserInputException FailedToParseUserCommand()
    {
        return new InvalidUserInputException(UserMessages.FailedToParseUserCommand);
    }
}