using Kysect.Shreks.Application.Queue;
using Kysect.Shreks.Core.Queue;
using Kysect.Shreks.Core.Queue.Visitors;
using Kysect.Shreks.Core.Study;
using Microsoft.Extensions.DependencyInjection;

namespace Kysect.Shreks.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection collection)
    {
        collection.AddQueue();

        return collection;
    }

    private static void AddQueue(this IServiceCollection collection)
    {
        collection.AddScoped<IQueueFilterVisitor<IQueryable<Submission>>, SubmissionQueryableFilterVisitor>();
        collection.AddScoped<ISubmissionEvaluatorVisitor<int>, SubmissionRatingEvaluatorVisitor>();
        collection.AddSingleton<IQueryExecutor, QueryExecutor>();
    }
}