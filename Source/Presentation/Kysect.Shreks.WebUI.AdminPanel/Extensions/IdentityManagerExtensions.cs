using Kysect.Shreks.WebUI.AdminPanel.Exceptions;
using Kysect.Shreks.WebUI.AdminPanel.Identity;
using Kysect.Shreks.WebUI.AdminPanel.Models;

namespace Kysect.Shreks.WebUI.AdminPanel.Extensions;

public static class IdentityManagerExtensions
{
    public static async ValueTask<UserIdentity> GetIdentityAsync(
        this IIdentityManager manager,
        CancellationToken cancellationToken)
    {
        var identity = await manager.FindIdentityAsync(cancellationToken);

        if (identity is null)
            throw new UnauthorizedException();

        return identity;
    }
}