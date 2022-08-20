using Kysect.Shreks.Application.Commands.Parsers;
using Microsoft.Extensions.DependencyInjection;

namespace Kysect.Shreks.Application.Commands.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationCommands(this IServiceCollection services)
    {
        services.AddSingleton<IShreksCommandParser, ShreksCommandParser>();
        
        return services;
    }
}