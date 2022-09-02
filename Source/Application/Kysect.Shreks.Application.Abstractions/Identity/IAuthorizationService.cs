namespace Kysect.Shreks.Application.Abstractions.Identity;

public interface IAuthorizationService
{
    Task AuthorizeAdminAsync(string username, CancellationToken cancellationToken);
}