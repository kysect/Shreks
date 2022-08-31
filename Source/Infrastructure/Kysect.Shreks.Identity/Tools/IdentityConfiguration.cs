namespace Kysect.Shreks.Identity.Tools;

public class IdentityConfiguration
{
    public string Secret { get; init; }
    
    public string Issuer { get; init; }
    
    public string Audience { get; init; }
    
    public int ExpiresHours { get; init; }
}