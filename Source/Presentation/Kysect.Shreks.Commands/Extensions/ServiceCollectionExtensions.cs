using Kysect.Shreks.Commands.Parsers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Kysect.Shreks.Commands.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPresentationCommands(this IServiceCollection collection)
    {
        collection.TryAddSingleton<ISubmissionCommandParser, SubmissionCommandParser>();
        return collection;
    }
}