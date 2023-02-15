namespace ITMO.Dev.ASAP.Identity.Tools;

public class IdentityConfiguration
{
    public string Secret { get; init; } = string.Empty;

    public string Issuer { get; init; } = string.Empty;

    public string Audience { get; init; } = string.Empty;

    public int ExpiresHours { get; init; }
}