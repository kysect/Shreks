using FluentSpreadsheets.GoogleSheets.Rendering;
using FluentSpreadsheets.Rendering;
using FluentSpreadsheets.SheetBuilders;
using Kysect.Shreks.Application.Abstractions.Google;
using Kysect.Shreks.Core.Formatters;
using Kysect.Shreks.Core.Users;
using Kysect.Shreks.Integration.Google.Providers;
using Kysect.Shreks.Integration.Google.Segments;
using Kysect.Shreks.Integration.Google.Segments.Factories;
using Kysect.Shreks.Integration.Google.Sheets;
using Kysect.Shreks.Integration.Google.Tools;
using Microsoft.Extensions.DependencyInjection;

namespace Kysect.Shreks.Integration.Google.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSheetServices(this IServiceCollection serviceCollection)
    {
        return serviceCollection
            .AddSheetSegments()
            .AddSingleton<IStudentComponentFactory, StudentComponentFactory>()
            .AddSingleton<ISheet<Points>, PointsSheet>()
            .AddSingleton<ISheet<Queue>, QueueSheet>()
            .AddSingleton<ISheetController, SheetController>()
            .AddSingleton<ISheetBuilder, SheetBuilder>()
            .AddSingleton<IComponentRenderer<GoogleSheetRenderCommand>, GoogleSheetComponentRenderer>();
    }

    private static IServiceCollection AddSheetSegments(this IServiceCollection serviceCollection)
    {
        return serviceCollection
            .AddSingleton<PointsStudentSegment>()
            .AddSingleton<AssignmentPointsSegment>()
            .AddSingleton<TotalPointsSegment>()
            .AddSingleton<QueueStudentSegment>()
            .AddSingleton<AssignmentDataSegment>();
    }
}