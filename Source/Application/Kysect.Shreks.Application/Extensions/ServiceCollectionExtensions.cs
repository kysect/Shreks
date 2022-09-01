using Kysect.Shreks.Application.Factory;
using Kysect.Shreks.Application.Tools;
using Kysect.Shreks.Core.Queue;
using Microsoft.Extensions.DependencyInjection;

namespace Kysect.Shreks.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationConfiguration(this IServiceCollection collection)
    {
        collection.AddScoped<ISubmissionFactory, SubmissionFactory>();
        return collection.AddSingleton<IQueryExecutor, QueryExecutor>();
    }
}