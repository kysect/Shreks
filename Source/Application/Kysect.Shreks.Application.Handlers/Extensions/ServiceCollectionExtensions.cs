using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Kysect.Shreks.Application.Handlers.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddHandlers(this IServiceCollection collection)
    {
        collection.AddMediatR(typeof(IAssemblyMarker));

        return collection;
    }
}