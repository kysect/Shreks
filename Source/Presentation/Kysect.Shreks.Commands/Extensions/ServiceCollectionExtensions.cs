using Kysect.Shreks.Commands.Parsers;
using Kysect.Shreks.Commands.Tools;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Kysect.Shreks.Commands.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPresentationCommands(this IServiceCollection collection)
    {
        collection.TryAddSingleton<ISubmissionCommandParser, SubmissionCommandParser>();
        collection.AddScoped<IDefaultSubmissionProvider, DefaultSubmissionProvider>();
        return collection;
    }
}