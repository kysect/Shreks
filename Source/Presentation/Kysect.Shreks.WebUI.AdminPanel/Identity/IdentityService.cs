using Kysect.Shreks.WebApi.Sdk;
using Kysect.Shreks.WebUI.AdminPanel.Models;

namespace Kysect.Shreks.WebUI.AdminPanel.Identity;

public class IdentityService : IIdentityService
{
    private readonly IIdentityManager _identityManager;
    private readonly IdentityClient _identityClient;

    public IdentityService(IIdentityManager identityManager, IdentityClient identityClient)
    {
        _identityManager = identityManager;
        _identityClient = identityClient;
    }

    public async Task LoginAsync(string username, string password, CancellationToken cancellationToken)
    {
        var request = new LoginRequest { Username = username, Password = password, };
        var response = await _identityClient.LoginAsync(request, cancellationToken);
        var identity = new UserIdentity(response.Token, response.Expires);

        await _identityManager.UpdateIdentityAsync(identity, cancellationToken);
    }
}