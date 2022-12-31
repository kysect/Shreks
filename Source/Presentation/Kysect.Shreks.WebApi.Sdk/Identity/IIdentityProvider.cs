using Kysect.Shreks.WebApi.Sdk.Models;

namespace Kysect.Shreks.WebApi.Sdk.Identity;

public interface IIdentityProvider
{
    ValueTask<UserIdentity?> FindIdentityAsync(CancellationToken cancellationToken = default);
}