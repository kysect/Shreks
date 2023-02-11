using ITMO.Dev.ASAP.WebApi.Sdk.Models;

namespace ITMO.Dev.ASAP.WebApi.Sdk.Identity;

public interface IIdentityProvider
{
    ValueTask<UserIdentity?> FindIdentityAsync(CancellationToken cancellationToken = default);
}