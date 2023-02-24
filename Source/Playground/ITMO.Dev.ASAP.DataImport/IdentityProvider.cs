using ITMO.Dev.ASAP.WebApi.Sdk.Identity;
using ITMO.Dev.ASAP.WebApi.Sdk.Models;

namespace ITMO.Dev.ASAP.DataImport;

public class IdentityProvider : IIdentityProvider
{
    public UserIdentity? UserIdentity { get; set; }

    public ValueTask<UserIdentity?> FindIdentityAsync(CancellationToken cancellationToken = default)
        => ValueTask.FromResult(UserIdentity);
}