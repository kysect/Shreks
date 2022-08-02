using Kysect.Shreks.Core.Formatters;
using Kysect.Shreks.Core.Study;
using Kysect.Shreks.Integration.Google.Converters;
using Kysect.Shreks.Integration.Google.Factories;
using Kysect.Shreks.Integration.Google.Models;
using Kysect.Shreks.Integration.Google.Sheets;
using Microsoft.Extensions.DependencyInjection;

namespace Kysect.Shreks.Integration.Google.Extensions;

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