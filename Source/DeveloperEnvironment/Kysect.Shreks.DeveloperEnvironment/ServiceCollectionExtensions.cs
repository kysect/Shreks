using Microsoft.Extensions.DependencyInjection;

namespace Kysect.Shreks.DeveloperEnvironment;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDeveloperEnvironmentSeeding(this IServiceCollection services)
    {
        services.AddScoped<DeveloperEnvironmentSeeder>();

        return services;
    }
}