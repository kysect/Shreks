using Kysect.Shreks.Application.Extensions;
using Kysect.Shreks.Application.Google.Extensions;
using Kysect.Shreks.Application.Handlers.Extensions;
using Kysect.Shreks.Presentation.GitHub.Extensions;
using Serilog;

namespace Kysect.Shreks.Playground.Github.Extensions;

internal static class ServiceCollectionExtensions
{
    internal static IServiceCollection AddPlaygroundDependencies(this IServiceCollection servicesCollection)
    {
        return servicesCollection.AddApplicationConfiguration()
            .AddHandlers()
            .AddGithubPresentation()
            .AddDummyGoogleIntegration()
            .AddLogging(logBuilder => logBuilder.AddSerilog());
    }
}