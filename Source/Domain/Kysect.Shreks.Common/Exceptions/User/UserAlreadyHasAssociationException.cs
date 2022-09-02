namespace Kysect.Shreks.Common.Exceptions.User;

public class UserAlreadyHasAssociationException : ShreksDomainException
{
    public UserAlreadyHasAssociationException(string associationName)
        : base($"User already has a {associationName} association") { }
}