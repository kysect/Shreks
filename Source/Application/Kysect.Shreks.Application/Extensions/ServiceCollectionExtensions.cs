using FluentChaining;
using FluentChaining.Configurators;
using Kysect.Shreks.Application.Abstractions.Permissions;
using Kysect.Shreks.Application.Abstractions.SubjectCourses;
using Kysect.Shreks.Application.Abstractions.Submissions;
using Kysect.Shreks.Application.Dto.Querying;
using Kysect.Shreks.Application.Queries;
using Kysect.Shreks.Application.Services;
using Kysect.Shreks.Application.Tools;
using Kysect.Shreks.Application.Validators;
using Kysect.Shreks.Core.Queue;
using Kysect.Shreks.Core.Study;
using Kysect.Shreks.Core.Users;
using Microsoft.Extensions.DependencyInjection;

namespace Kysect.Shreks.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationConfiguration(this IServiceCollection collection)
    {
        collection.AddSingleton<IQueryExecutor, QueryExecutor>();
        collection.AddScoped<IPermissionValidator, PermissionValidator>();
        collection.AddScoped<ISubjectCourseService, SubjectCourseService>();
        collection.AddScoped<ISubmissionWorkflowService, SubmissionWorkflowService>();

        collection.AddQueryChains();

        return collection;
    }

    private static void AddQueryChains(this IServiceCollection collection)
    {
        collection.AddEntityQuery<Student, StudentQueryParameter>();
        collection.AddEntityQuery<StudentGroup, GroupQueryParameter>();

        collection
            .AddFluentChaining(x => x.ChainLifetime = ServiceLifetime.Singleton)
            .AddQueryChain<Student, StudentQueryParameter>()
            .AddQueryChain<StudentGroup, GroupQueryParameter>();
    }

    private static IChainConfigurator AddQueryChain<TValue, TParameter>(this IChainConfigurator configurator)
    {
        return configurator.AddChain<EntityQueryRequest<TValue, TParameter>, IQueryable<TValue>>(x => x
            .ThenFromAssemblies(typeof(IAssemblyMarker))
            .FinishWith((r, _) => r.Query));
    }

    private static void AddEntityQuery<TValue, TParameter>(this IServiceCollection collection)
    {
        collection.AddSingleton<IEntityQuery<TValue, TParameter>, EntityQueryAdapter<TValue, TParameter>>();
    }
}