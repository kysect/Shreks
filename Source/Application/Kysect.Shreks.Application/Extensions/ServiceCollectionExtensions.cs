using FluentChaining;
using Kysect.Shreks.Application.Abstractions.Permissions;
using Kysect.Shreks.Application.Dto.Users;
using Kysect.Shreks.Application.Queries;
using Kysect.Shreks.Application.Tools;
using Kysect.Shreks.Application.Validators;
using Kysect.Shreks.Core.Queue;
using Kysect.Shreks.Core.Users;
using Microsoft.Extensions.DependencyInjection;

namespace Kysect.Shreks.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationConfiguration(this IServiceCollection collection)
    {
        collection.AddSingleton<IQueryExecutor, QueryExecutor>();
        collection.AddScoped<IPermissionValidator, PermissionValidator>();

        collection.AddStudentQuery();

        return collection;
    }

    private static void AddStudentQuery(this IServiceCollection collection)
    {
        collection.AddSingleton<
            IEntityQuery<Student, StudentQueryParameter>,
            EntityQueryAdapter<Student, StudentQueryParameter>>();

        collection
            .AddFluentChaining(x => x.ChainLifetime = ServiceLifetime.Singleton)
            .AddChain<EntityQueryRequest<Student, StudentQueryParameter>, IQueryable<Student>>(x => x
                .ThenFromAssemblies(typeof(IAssemblyMarker))
                .FinishWith((r, _) => r.Query));
    }
}