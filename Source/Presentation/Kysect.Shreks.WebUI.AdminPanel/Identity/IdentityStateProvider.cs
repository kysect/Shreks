using Kysect.Shreks.WebApi.Sdk.Models;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace Kysect.Shreks.WebUI.AdminPanel.Identity;

public class IdentityStateProvider : AuthenticationStateProvider
{
    private readonly IIdentityManager _identityManager;
    private readonly IIdentityService _identityService;

    public IdentityStateProvider(IIdentityManager identityManager, IIdentityService identityService)
    {
        _identityManager = identityManager;
        _identityService = identityService;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        UserIdentity? userIdentity = await _identityManager.FindIdentityAsync();

        var identity = new ClaimsIdentity();

        if (userIdentity is not null)
        {
            Claim[] claims = Array.Empty<Claim>();
            identity = new ClaimsIdentity(claims, "Shreks Identity");
        }

        return new AuthenticationState(new ClaimsPrincipal(identity));
    }

    public async Task LoginAsync(string username, string password, CancellationToken cancellationToken)
    {
        await _identityService.LoginAsync(username, password, cancellationToken);
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    public async Task LogoutAsync(CancellationToken cancellationToken)
    {
        await _identityManager.RemoveIdentityAsync(cancellationToken);
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    public void Update()
    {
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }
}