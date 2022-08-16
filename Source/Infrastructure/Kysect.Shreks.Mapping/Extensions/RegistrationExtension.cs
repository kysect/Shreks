using Microsoft.Extensions.DependencyInjection;

namespace Kysect.Shreks.Mapping.Extensions;

public static class RegistrationExtension
{
    public static ServiceCollection AddMappingConfiguration(this ServiceCollection collection)
    {
        collection.AddAutoMapper(typeof(IAssemblyMarker));
    
    	return collection;
    }
}