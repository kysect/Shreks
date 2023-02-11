using ITMO.Dev.ASAP.Commands.Parsers;
using ITMO.Dev.ASAP.Commands.Tools;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ITMO.Dev.ASAP.Commands.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPresentationCommands(this IServiceCollection collection)
    {
        collection.TryAddSingleton<ISubmissionCommandParser, SubmissionCommandParser>();
        collection.AddScoped<IDefaultSubmissionProvider, DefaultSubmissionProvider>();
        return collection;
    }
}