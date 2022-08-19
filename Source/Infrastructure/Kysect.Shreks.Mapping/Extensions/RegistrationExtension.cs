using Microsoft.Extensions.DependencyInjection;

namespace Kysect.Shreks.Mapping.Extensions;

public static class RegistrationExtension
{
    public static IServiceCollection AddMappingConfiguration(this IServiceCollection collection)
    {
        collection.AddAutoMapper(typeof(IAssemblyMarker));
    
    	return collection;
    }
}