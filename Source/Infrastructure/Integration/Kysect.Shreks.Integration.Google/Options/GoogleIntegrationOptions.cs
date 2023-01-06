using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Kysect.Shreks.Integration.Google.Providers;
using Microsoft.Extensions.DependencyInjection;

namespace Kysect.Shreks.Integration.Google.Options;

public class GoogleIntegrationOptions
{
    private static readonly string[] AccessScopes = { SheetsService.Scope.Spreadsheets, DriveService.Scope.Drive };

    private readonly IServiceCollection _collection;

    public GoogleIntegrationOptions(IServiceCollection collection)
    {
        _collection = collection;
    }

    public GoogleIntegrationOptions ConfigureGoogleCredentials(GoogleCredential credentials)
    {
        ArgumentNullException.ThrowIfNull(credentials);

        var initializer = new BaseClientService.Initializer
        {
            HttpClientInitializer = credentials.CreateScoped(AccessScopes),
        };

        _collection
            .AddSingleton(new SheetsService(initializer))
            .AddSingleton(new DriveService(initializer));

        return this;
    }

    public GoogleIntegrationOptions ConfigureDriveId(string driveId)
    {
        var driveParentProvider = new DriveParentProvider(driveId);
        _collection.AddSingleton<ITablesParentsProvider>(driveParentProvider);

        return this;
    }
}