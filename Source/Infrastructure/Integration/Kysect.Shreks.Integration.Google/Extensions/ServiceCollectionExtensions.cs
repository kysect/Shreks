using FluentSpreadsheets.GoogleSheets.Rendering;
using FluentSpreadsheets.Rendering;
using FluentSpreadsheets.SheetBuilders;
using Kysect.Shreks.Application.Abstractions.Google;
using Kysect.Shreks.Integration.Google.Factories;
using Kysect.Shreks.Integration.Google.Segments;
using Kysect.Shreks.Integration.Google.Sheets;
using Kysect.Shreks.Integration.Google.Tools;
using Microsoft.Extensions.DependencyInjection;

namespace Kysect.Shreks.Integration.Google.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSheetServices(this IServiceCollection serviceCollection)
    {
        return serviceCollection
            .AddSingleton<IStudentComponentFactory, StudentComponentFactory>()
            .AddSingleton<ISheetDataFactory<Points>, PointsSheetDataFactory>()
            .AddSingleton<ISheetDataFactory<Queue>, QueueSheetDataFactory>()
            .AddSingleton<ISheet<Points>, PointsSheet>()
            .AddSingleton<ISheet<Queue>, QueueSheet>()
            .AddSingleton<ISheetController, SheetController>()
            .AddSingleton<ISheetBuilder, SheetBuilder>()
            .AddSingleton<IComponentRenderer<GoogleSheetRenderCommand>, GoogleSheetComponentRenderer>();
    }
}