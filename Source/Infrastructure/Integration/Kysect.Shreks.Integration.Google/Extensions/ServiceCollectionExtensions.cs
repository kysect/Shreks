using FluentSpreadsheets.GoogleSheets.Rendering;
using FluentSpreadsheets.Rendering;
using FluentSpreadsheets.SheetBuilders;
using Kysect.Shreks.Application.Abstractions.Google;
using Kysect.Shreks.Application.Dto.Tables;
using Kysect.Shreks.Integration.Google.Factories;
using Kysect.Shreks.Integration.Google.Options;
using Kysect.Shreks.Integration.Google.Sheets;
using Kysect.Shreks.Integration.Google.Tools;
using Microsoft.Extensions.DependencyInjection;

namespace Kysect.Shreks.Integration.Google.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGoogleIntegration(
        this IServiceCollection serviceCollection,
        Action<GoogleIntegrationOptions> action)
    {
        ArgumentNullException.ThrowIfNull(action);

        var options = new GoogleIntegrationOptions(serviceCollection);
        action.Invoke(options);

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
            .AddSingleton(p => new GoogleTableUpdateWorker(p.GetRequiredService<GoogleTableAccessor>()))
            .AddSingleton<ITableUpdateQueue>(p => p.GetRequiredService<GoogleTableUpdateWorker>())
            .AddHostedService<GoogleTableUpdateWorker>();
    }
}