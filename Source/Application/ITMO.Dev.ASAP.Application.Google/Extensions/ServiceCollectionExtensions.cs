using FluentSpreadsheets.GoogleSheets.Factories;
using FluentSpreadsheets.GoogleSheets.Rendering;
using FluentSpreadsheets.Rendering;
using FluentSpreadsheets.Tables;
using ITMO.Dev.ASAP.Application.Abstractions.Formatters;
using ITMO.Dev.ASAP.Application.Abstractions.Google;
using ITMO.Dev.ASAP.Application.Abstractions.Google.Sheets;
using ITMO.Dev.ASAP.Application.Dto.SubjectCourses;
using ITMO.Dev.ASAP.Application.Dto.Tables;
using ITMO.Dev.ASAP.Application.Google.Services;
using ITMO.Dev.ASAP.Application.Google.Workers;
using ITMO.Dev.ASAP.Integration.Google.Models;
using ITMO.Dev.ASAP.Integration.Google.Options;
using ITMO.Dev.ASAP.Integration.Google.Providers;
using ITMO.Dev.ASAP.Integration.Google.Sheets;
using ITMO.Dev.ASAP.Integration.Google.Tables;
using ITMO.Dev.ASAP.Integration.Google.Tools;
using ITMO.Dev.ASAP.Integration.Google.Tools.Comparers;
using Microsoft.Extensions.DependencyInjection;

namespace ITMO.Dev.ASAP.Application.Google.Extensions;

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
            .AddSingleton<ISheet<CourseStudentsDto>, PointsSheet>()
            .AddSingleton<ISheet<SubjectCoursePointsDto>, LabsSheet>()
            .AddSingleton<ISheet<SubmissionsQueueDto>, QueueSheet>()
            .AddSingleton<ITable<CourseStudentsDto>, PointsTable>()
            .AddSingleton<ITable<SubjectCoursePointsDto>, LabsTable>()
            .AddSingleton<ITable<SubmissionsQueueDto>, QueueTable>()
            .AddSingleton<ISheetManagementService, SheetManagementService>()
            .AddSingleton<ISpreadsheetManagementService, SpreadsheetManagementService>()
            .AddSingleton<IRenderCommandFactory, RenderCommandFactory>()
            .AddSingleton<ISheetTitleComparer, SheetTitleComparer>()
            .AddScoped<ISubjectCourseTableService, SubjectCourseTableService>()
            .AddSingleton<IComponentRenderer<GoogleSheetRenderCommand>, GoogleSheetComponentRenderer>()
            .AddGoogleTableUpdateWorker()
            .AddGoogleFormatter();
    }

    public static IServiceCollection AddDummyGoogleIntegration(this IServiceCollection serviceCollection)
    {
        return serviceCollection
            .AddSingleton<TableUpdateQueue>()
            .AddSingleton<ITableUpdateQueue>(p => p.GetRequiredService<TableUpdateQueue>())
            .AddSingleton<IUserFullNameFormatter, UserFullNameFormatter>()
            .AddSingleton<ICultureInfoProvider, RuCultureInfoProvider>();
    }

    private static IServiceCollection AddGoogleTableUpdateWorker(this IServiceCollection serviceCollection)
    {
        return serviceCollection
            .AddSingleton<TableUpdateQueue>()
            .AddSingleton<ITableUpdateQueue>(p => p.GetRequiredService<TableUpdateQueue>())
            .AddHostedService<GoogleTableUpdateWorker>();
    }

    private static IServiceCollection AddGoogleFormatter(this IServiceCollection serviceCollection)
    {
        return serviceCollection
            .AddSingleton<IUserFullNameFormatter, UserFullNameFormatter>()
            .AddSingleton<ICultureInfoProvider, RuCultureInfoProvider>();
    }
}