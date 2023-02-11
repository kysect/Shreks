using ITMO.Dev.ASAP.Common.Resources;

namespace ITMO.Dev.ASAP.Common.Exceptions;

public class UnauthorizedException : DomainException
{
    public UnauthorizedException(string? message) : base(message) { }

    public UnauthorizedException()
        : base(UserMessages.UnauthorizedExceptionMessage) { }

    public static UnauthorizedException DoesNotHavePermissionForActivateSubmission()
    {
        return new UnauthorizedException(UserMessages.UserDoesNotHavePermissionForActivateSubmission);
    }

    public static UnauthorizedException DoesNotHavePermissionForChangeSubmission()
    {
        return new UnauthorizedException(UserMessages.UserDoesNotHavePermissionForActivateSubmission);
    }
}