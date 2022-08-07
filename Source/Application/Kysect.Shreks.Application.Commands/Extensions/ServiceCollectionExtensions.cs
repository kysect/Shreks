using Kysect.Shreks.Application.Commands.Commands;
using Microsoft.Extensions.DependencyInjection;

namespace Kysect.Shreks.Application.Commands.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCommandParser(this IServiceCollection services)
    {
        services.AddSingleton<ShreksCommandParser>();
        
        return services;
    }
}