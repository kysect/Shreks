using Kysect.Shreks.WebUI.AdminPanel.Models;

namespace Kysect.Shreks.WebUI.AdminPanel.Identity;

public interface IIdentityManager
{
    ValueTask<UserIdentity?> FindIdentityAsync(CancellationToken cancellationToken);
    
    ValueTask UpdateIdentityAsync(UserIdentity userIdentity, CancellationToken cancellationToken);
    
    ValueTask RemoveIdentityAsync(CancellationToken cancellationToken);

    ValueTask<bool> HasIdentityAsync(CancellationToken cancellationToken);
}