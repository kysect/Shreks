using Kysect.Shreks.WebApi.Sdk.Exceptions;
using Kysect.Shreks.WebApi.Sdk.Models;
using Kysect.Shreks.WebUI.AdminPanel.Identity;

namespace Kysect.Shreks.WebUI.AdminPanel.Extensions;

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