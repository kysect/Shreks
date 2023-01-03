using Kysect.Shreks.WebApi.Sdk.Models;
using System.Net.Http.Headers;

namespace Kysect.Shreks.WebApi.Sdk.Identity;

public class AuthorizationMessageHandlerDecorator : DelegatingHandler
{
    private readonly IIdentityProvider _identityProvider;

    public AuthorizationMessageHandlerDecorator(IIdentityProvider identityProvider)
    {
        _identityProvider = identityProvider;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        UserIdentity? identity = await _identityProvider.FindIdentityAsync(cancellationToken);

        if (identity is not null)
            request.Headers.Authorization = AuthenticationHeaderValue.Parse($"Bearer {identity.Token}");

        return await base.SendAsync(request, cancellationToken);
    }
}