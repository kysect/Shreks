using ITMO.Dev.ASAP.WebApi.Sdk.Exceptions;
using ITMO.Dev.ASAP.WebApi.Sdk.Models;
using ITMO.Dev.ASAP.WebUI.AdminPanel.Identity;

namespace ITMO.Dev.ASAP.WebUI.AdminPanel.Extensions;

public static class IdentityManagerExtensions
{
    public static async ValueTask<UserIdentity> GetIdentityAsync(
        this IIdentityManager manager,
        CancellationToken cancellationToken)
    {
        UserIdentity? identity = await manager.FindIdentityAsync(cancellationToken);

        if (identity is null)
            throw new UnauthorizedException();

        return identity;
    }
}