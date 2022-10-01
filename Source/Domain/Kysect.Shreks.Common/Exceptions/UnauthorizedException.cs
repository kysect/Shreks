using Kysect.Shreks.Common.Resources;

namespace Kysect.Shreks.Common.Exceptions;

public class UnauthorizedException : ShreksDomainException
{
    public UnauthorizedException(string? message) : base(message) { }

    public UnauthorizedException() : base(UserMessages.UnauthorizedExceptionMessage)
    {
    }

    public static UnauthorizedException DoesNotHavePermissionForActivateSubmission()
    {
        return new UnauthorizedException(UserMessages.UserDoesNotHavePermissionForActivateSubmission);
    }
    public static UnauthorizedException DoesNotHavePermissionForChangeSubmission()
    {
        return new UnauthorizedException(UserMessages.UserDoesNotHavePermissionForActivateSubmission);
    }
}