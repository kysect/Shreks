using FluentChaining;
using FluentChaining.Configurators;
using ITMO.Dev.ASAP.Application.Abstractions.Permissions;
using ITMO.Dev.ASAP.Application.Abstractions.SubjectCourses;
using ITMO.Dev.ASAP.Application.Abstractions.Submissions;
using ITMO.Dev.ASAP.Application.Dto.Querying;
using ITMO.Dev.ASAP.Application.Queries;
using ITMO.Dev.ASAP.Application.Services;
using ITMO.Dev.ASAP.Application.Tools;
using ITMO.Dev.ASAP.Application.Validators;
using ITMO.Dev.ASAP.Core.Queue;
using ITMO.Dev.ASAP.Core.Study;
using ITMO.Dev.ASAP.Core.Users;
using Microsoft.Extensions.DependencyInjection;

namespace ITMO.Dev.ASAP.Application.Extensions;

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