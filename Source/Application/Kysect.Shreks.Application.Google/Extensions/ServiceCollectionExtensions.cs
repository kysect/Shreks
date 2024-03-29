﻿using FluentSpreadsheets.GoogleSheets.Factories;
using FluentSpreadsheets.GoogleSheets.Rendering;
using FluentSpreadsheets.Rendering;
using FluentSpreadsheets.Tables;
using Kysect.Shreks.Application.Abstractions.Formatters;
using Kysect.Shreks.Application.Abstractions.Google;
using Kysect.Shreks.Application.Abstractions.Google.Sheets;
using Kysect.Shreks.Application.Dto.SubjectCourses;
using Kysect.Shreks.Application.Dto.Tables;
using Kysect.Shreks.Application.Google.Services;
using Kysect.Shreks.Application.Google.Workers;
using Kysect.Shreks.Integration.Google.Models;
using Kysect.Shreks.Integration.Google.Options;
using Kysect.Shreks.Integration.Google.Providers;
using Kysect.Shreks.Integration.Google.Sheets;
using Kysect.Shreks.Integration.Google.Tables;
using Kysect.Shreks.Integration.Google.Tools;
using Kysect.Shreks.Integration.Google.Tools.Comparers;
using Microsoft.Extensions.DependencyInjection;

namespace Kysect.Shreks.Application.Google.Extensions;

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