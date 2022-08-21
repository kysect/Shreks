using FluentSpreadsheets.GoogleSheets.Rendering;
using FluentSpreadsheets.Rendering;
using FluentSpreadsheets.SheetBuilders;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Util.Store;
using Kysect.Shreks.Application.Abstractions.Formatters;
using Kysect.Shreks.Application.Abstractions.Google;
using Kysect.Shreks.Application.Dto.Tables;
using Kysect.Shreks.Integration.Google.Factories;
using Kysect.Shreks.Integration.Google.Providers;
using Kysect.Shreks.Integration.Google.Sheets;
using Kysect.Shreks.Integration.Google.Tools;
using Microsoft.Extensions.DependencyInjection;

namespace Kysect.Shreks.Integration.Google.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGoogleCredentialsFromWeb(this IServiceCollection serviceCollection)
    {
        var stream = new FileStream("client_secrets.json", FileMode.Open, FileAccess.Read);
        var secrets = GoogleClientSecrets.FromStream(stream);

        UserCredential credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
            secrets.Secrets,
            GoogleRequiredScopes.GetRequiredScopes(),
            "user",
            CancellationToken.None,
            new FileDataStore("token.json", true)).Result;

        var initializer = new BaseClientService.Initializer
        {
            HttpClientInitializer = credential
        };

        return serviceCollection
            .AddSingleton(initializer);
    }

    public static IServiceCollection AddGoogleIntegration(this IServiceCollection serviceCollection)
    {
        return serviceCollection
            .AddSingleton<IStudentComponentFactory, StudentComponentFactory>()
            .AddSingleton<ISheetComponentFactory<CoursePointsDto>, PointsSheetComponentFactory>()
            .AddSingleton<ISheetComponentFactory<SubmissionsQueueDto>, QueueSheetComponentFactory>()
            .AddSingleton<ISheet<CoursePointsDto>, PointsSheet>()
            .AddSingleton<ISheet<SubmissionsQueueDto>, QueueSheet>()
            .AddSingleton<ISheetManagementService, SheetManagementService>()
            .AddSingleton<ISheetBuilder, SheetBuilder>()
            .AddSingleton<IComponentRenderer<GoogleSheetRenderCommand>, GoogleSheetComponentRenderer>()
            .AddSingleton<GoogleTableAccessor>()
            .AddGoogleTableUpdateWorker()
            .AddGoogleFormatter()
            .AddGoogleServices();
    }

    private static IServiceCollection AddGoogleTableUpdateWorker(this IServiceCollection serviceCollection)
    {
        return serviceCollection
            .AddSingleton(p => new GoogleTableUpdateWorker(p.GetRequiredService<GoogleTableAccessor>()))
            .AddSingleton<ITableUpdateQueue>(p => p.GetRequiredService<GoogleTableUpdateWorker>())
            .AddHostedService<GoogleTableUpdateWorker>();
    }

    private static IServiceCollection AddGoogleFormatter(this IServiceCollection serviceCollection)
    {
        return serviceCollection
            .AddSingleton<IUserFullNameFormatter, UserFullNameFormatter>()
            .AddSingleton<ICultureInfoProvider, RuCultureInfoProvider>();
    }

    private static IServiceCollection AddGoogleServices(this IServiceCollection serviceCollection)
    {
        return serviceCollection
            .AddSingleton(p => new SheetsService(p.GetRequiredService<BaseClientService.Initializer>()))
            .AddSingleton(p => new DriveService(p.GetRequiredService<BaseClientService.Initializer>()));
    }
}