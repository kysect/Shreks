using Blazored.LocalStorage;
using Kysect.Shreks.WebUI.AdminPanel.Models;

namespace Kysect.Shreks.WebUI.AdminPanel.Identity;

public class IdentityManager : IIdentityManager
{
    private const string IdentityKey = "ShreksIdentity";
    private readonly ILocalStorageService _storage;

    public IdentityManager(ILocalStorageService storage)
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
        var hasIdentity = await _storage.ContainKeyAsync(IdentityKey, cancellationToken);

        if (!hasIdentity)
            return false;

        var identity = await _storage.GetItemAsync<UserIdentity>(IdentityKey, cancellationToken);

        var now = DateTime.UtcNow;
        Console.WriteLine(identity.ExpirationDateTime);
        Console.WriteLine(now);
        return identity.ExpirationDateTime > now;
    }
}