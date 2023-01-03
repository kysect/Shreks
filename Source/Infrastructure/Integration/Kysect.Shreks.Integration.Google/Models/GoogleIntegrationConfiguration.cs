namespace Kysect.Shreks.Integration.Google.Models;

public class GoogleIntegrationConfiguration
{
    public string ClientSecrets { get; set; } = string.Empty;
    public string GoogleDriveId { get; set; } = string.Empty;
    public bool EnableGoogleIntegration { get; set; }
}