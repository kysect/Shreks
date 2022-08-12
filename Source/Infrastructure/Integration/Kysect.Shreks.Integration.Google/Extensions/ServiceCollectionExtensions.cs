using FluentSpreadsheets.GoogleSheets.Rendering;
using FluentSpreadsheets.Rendering;
using FluentSpreadsheets.SheetBuilders;
using Kysect.Shreks.Application.Abstractions.Google;
using Kysect.Shreks.Integration.Google.Factories;
using Kysect.Shreks.Integration.Google.Sheets;
using Kysect.Shreks.Integration.Google.Tools;
using Microsoft.Extensions.DependencyInjection;

namespace Kysect.Shreks.Integration.Google.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGoogleIntegration(this IServiceCollection serviceCollection)
    {
        return serviceCollection
            .AddSingleton<IStudentComponentFactory, StudentComponentFactory>()
            .AddSingleton<ISheetComponentFactory<Points>, PointsSheetComponentFactory>()
            .AddSingleton<ISheetComponentFactory<Queue>, QueueSheetComponentFactory>()
            .AddSingleton<ISheet<Points>, PointsSheet>()
            .AddSingleton<ISheet<Queue>, QueueSheet>()
            .AddSingleton<ISheetManagementService, SheetManagementService>()
            .AddSingleton<ISheetBuilder, SheetBuilder>()
            .AddSingleton<IComponentRenderer<GoogleSheetRenderCommand>, GoogleSheetComponentRenderer>()
            .AddSingleton<IGoogleTableAccessor, GoogleTableAccessor>();
    }
}