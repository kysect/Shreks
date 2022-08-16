using Microsoft.Extensions.DependencyInjection;

namespace Kysect.Shreks.Mapping.Extensions;

public static class RegistrationExtension
{
    public static void AddMapping(this ServiceCollection collection)
    {
        collection.AddAutoMapper(typeof(IAssemblyMarker));
    }
}