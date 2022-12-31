using Blazored.LocalStorage;
using Kysect.Shreks.WebApi.Sdk.Models;

namespace Kysect.Shreks.WebUI.AdminPanel.Identity;

public class LocalStorageIdentityManager : IIdentityManager
{
    private const string IdentityKey = "ShreksIdentity";
    private readonly ILocalStorageService _storage;

    public LocalStorageIdentityManager(ILocalStorageService storage)
    {
        _storage = storage;
    }

    public async ValueTask<UserIdentity?> FindIdentityAsync(CancellationToken cancellationToken)
    {
        var identity = await _storage.GetItemAsync<UserIdentity>(IdentityKey, cancellationToken);

        if (identity is null)
            return null;

        if (identity.ExpirationDateTime > DateTime.UtcNow)
            return identity;

        await _storage.RemoveItemAsync(IdentityKey, cancellationToken);
        return null;
    }

    public ValueTask UpdateIdentityAsync(UserIdentity userIdentity, CancellationToken cancellationToken)
        => _storage.SetItemAsync(IdentityKey, userIdentity, cancellationToken);

    public ValueTask RemoveIdentityAsync(CancellationToken cancellationToken)
        => _storage.RemoveItemAsync(IdentityKey, cancellationToken);

    public async ValueTask<bool> HasIdentityAsync(CancellationToken cancellationToken)
    {
        var identity = await _storage.GetItemAsync<UserIdentity>(IdentityKey, cancellationToken);

        if (identity is null)
            return false;

        var now = DateTime.UtcNow;

        return identity.ExpirationDateTime > now;
    }
}