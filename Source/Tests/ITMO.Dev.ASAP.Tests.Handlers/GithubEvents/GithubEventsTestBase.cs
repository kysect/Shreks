using ITMO.Dev.ASAP.Application.Extensions;
using ITMO.Dev.ASAP.Application.Google.Extensions;
using ITMO.Dev.ASAP.Core.Submissions;
using ITMO.Dev.ASAP.Seeding.Options;
using Microsoft.Extensions.DependencyInjection;

namespace ITMO.Dev.ASAP.Tests.Handlers.GithubEvents;

public class GithubEventsTestBase : TestBase
{
    protected override void ConfigureServices(IServiceCollection collection)
    {
        collection.AddApplicationConfiguration();
        collection.AddDummyGoogleIntegration();
    }

    protected override void ConfigureSeeding(EntityGenerationOptions options)
    {
        options.ConfigureEntityGenerator<GithubSubmission>(x => x.Count = 50);
    }
}