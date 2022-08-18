using FluentSpreadsheets.GoogleSheets.Rendering;
using FluentSpreadsheets.Rendering;
using FluentSpreadsheets.SheetBuilders;
using Kysect.Shreks.Application.Abstractions.Google;
using Kysect.Shreks.Application.Abstractions.Google.Models;
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
            .AddSingleton<ISheetComponentFactory<CoursePoints>, PointsSheetComponentFactory>()
            .AddSingleton<ISheetComponentFactory<StudentsQueue>, QueueSheetComponentFactory>()
            .AddSingleton<ISheet<CoursePoints>, PointsSheet>()
            .AddSingleton<ISheet<StudentsQueue>, QueueSheet>()
            .AddSingleton<ISheetManagementService, SheetManagementService>()
            .AddSingleton<ISheetBuilder, SheetBuilder>()
            .AddSingleton<IComponentRenderer<GoogleSheetRenderCommand>, GoogleSheetComponentRenderer>()
            .AddSingleton<GoogleTableAccessor>()
            .AddSingleton(p => new GoogleTableAccessorWorker(p.GetRequiredService<GoogleTableAccessor>()))
            .AddSingleton<IGoogleTableAccessor>(p => p.GetRequiredService<GoogleTableAccessorWorker>())
            .AddHostedService<GoogleTableAccessorWorker>();
    }
}