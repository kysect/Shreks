using Kysect.Shreks.WebApi.Sdk.Identity;
using Kysect.Shreks.WebApi.Sdk.Models;

namespace Kysect.Shreks.WebUI.AdminPanel.Identity;

public interface IIdentityManager : IIdentityProvider
{
    ValueTask UpdateIdentityAsync(UserIdentity userIdentity, CancellationToken cancellationToken);

    ValueTask RemoveIdentityAsync(CancellationToken cancellationToken);

    ValueTask<bool> HasIdentityAsync(CancellationToken cancellationToken);
}