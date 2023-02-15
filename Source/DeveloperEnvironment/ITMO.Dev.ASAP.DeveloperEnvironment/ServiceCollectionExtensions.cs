namespace ITMO.Dev.ASAP.DeveloperEnvironment;

public static class ServiceCollectionExtensions
{
    public static void AddDeveloperEnvironment(this WebApplicationBuilder builder)
    {
        if (builder.Environment.IsDevelopment() || builder.Environment.IsStaging())
        {
            builder.Services.AddScoped<DeveloperEnvironmentSeeder>();
            builder.Services.AddMvc().AddApplicationPart(typeof(IAssemblyMarker).Assembly);
        }
    }
}