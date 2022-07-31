using Kysect.Shreks.Core.Formatters;
using Kysect.Shreks.Core.Study;
using Kysect.Shreks.GoogleIntegration.Converters;
using Kysect.Shreks.GoogleIntegration.Factories;
using Kysect.Shreks.GoogleIntegration.Models;
using Kysect.Shreks.GoogleIntegration.Sheets;
using Microsoft.Extensions.DependencyInjection;

namespace Kysect.Shreks.GoogleIntegration.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSheetServices(this IServiceCollection serviceCollection)
    {
        return serviceCollection
            .AddSingleton<ISheetFactory<PointsSheet>, PointsSheetFactory>()
            .AddSingleton<ISheetFactory<QueueSheet>, QueueSheetFactory>()
            .AddSingleton<IUserFullNameFormatter, UserFullNameFormatter>()
            .AddSingleton<ISheetRowConverter<StudentPointsArguments>, StudentPointsConverter>()
            .AddSingleton<ISheetRowConverter<Submission>, SubmissionsConverter>();
    }
}