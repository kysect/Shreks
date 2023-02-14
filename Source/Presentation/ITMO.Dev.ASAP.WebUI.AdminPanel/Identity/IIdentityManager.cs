using ITMO.Dev.ASAP.WebApi.Sdk.Identity;
using ITMO.Dev.ASAP.WebApi.Sdk.Models;

namespace ITMO.Dev.ASAP.WebUI.AdminPanel.Identity;

public interface IIdentityManager : IIdentityProvider
{
    ValueTask UpdateIdentityAsync(UserIdentity userIdentity, CancellationToken cancellationToken);

    ValueTask RemoveIdentityAsync(CancellationToken cancellationToken);

    ValueTask<bool> HasIdentityAsync(CancellationToken cancellationToken);
}