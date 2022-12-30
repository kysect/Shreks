using Kysect.Shreks.WebApi.Abstractions.Models.Identity;
using Kysect.Shreks.WebApi.Sdk.ControllerClients;
using Kysect.Shreks.WebApi.Sdk.Models;

namespace Kysect.Shreks.WebUI.AdminPanel.Identity;

public class IdentityService : IIdentityService
{
    private readonly IIdentityManager _identityManager;
    private readonly IIdentityClient _identityClient;

    public IdentityService(IIdentityManager identityManager, IIdentityClient identityClient)
    {
        _identityManager = identityManager;
        _identityClient = identityClient;
    }

    public async Task LoginAsync(string username, string password, CancellationToken cancellationToken)
    {
        var request = new LoginRequest(username, password);
        var response = await _identityClient.LoginAsync(request, cancellationToken);
        var identity = new UserIdentity(response.Token, response.Expires);

        await _identityManager.UpdateIdentityAsync(identity, cancellationToken);
    }
}