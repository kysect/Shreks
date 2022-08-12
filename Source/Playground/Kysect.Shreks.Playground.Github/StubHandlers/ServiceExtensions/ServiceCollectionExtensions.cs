using Kysect.Shreks.Core.Study;

namespace Kysect.Shreks.Playground.Github.StubHandlers.ServiceExtensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddStubHandlers(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton(new Submission(null!, null!, DateTime.Now, ""));

        return serviceCollection;
    }
}