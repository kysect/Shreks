using FluentSerialization.Extensions;
using FluentSerialization.Extensions.NewtonsoftJson;
using Microsoft.Extensions.DependencyInjection;

namespace ITMO.Dev.ASAP.Application.Dto.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDtoConfiguration(this IServiceCollection collection)
    {
        collection.AddFluentSerialization(typeof(IAssemblyMarker)).AddNewtonsoftJson();
        return collection;
    }
}