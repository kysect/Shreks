using Kysect.Shreks.WebUI.AdminPanel.Identity;
using System.Net.Http.Headers;

namespace Kysect.Shreks.WebUI.AdminPanel.Tools;

public class AuthorizationMessageHandlerDecorator : DelegatingHandler
{
    private readonly IIdentityManager _manager;

    public AuthorizationMessageHandlerDecorator(IIdentityManager manager)
    {
        _manager = manager;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var identity = await _manager.FindIdentityAsync(cancellationToken);

        if (identity is not null)
        {
            request.Headers.Authorization = AuthenticationHeaderValue.Parse($"Bearer {identity.Token}");
        }

        return await base.SendAsync(request, cancellationToken);
    }
}