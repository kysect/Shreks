using ITMO.Dev.ASAP.WebApi.Abstractions.Models.Identity;
using ITMO.Dev.ASAP.WebApi.Sdk.ControllerClients;
using ITMO.Dev.ASAP.WebApi.Sdk.Models;

namespace ITMO.Dev.ASAP.WebUI.AdminPanel.Identity;

public class IdentityService : IIdentityService
{
    private readonly IIdentityClient _identityClient;
    private readonly IIdentityManager _identityManager;

    public IdentityService(IIdentityManager identityManager, IIdentityClient identityClient)
    {
        _identityManager = identityManager;
        _identityClient = identityClient;
    }

    public async Task LoginAsync(string username, string password, CancellationToken cancellationToken)
    {
        var request = new LoginRequest(username, password);
        LoginResponse response = await _identityClient.LoginAsync(request, cancellationToken);
        var identity = new UserIdentity(response.Token, response.Expires);

        await _identityManager.UpdateIdentityAsync(identity, cancellationToken);
    }
}