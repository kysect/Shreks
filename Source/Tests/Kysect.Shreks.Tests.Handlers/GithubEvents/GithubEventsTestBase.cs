using Kysect.Shreks.Application.Extensions;
using Kysect.Shreks.Application.Google.Extensions;
using Kysect.Shreks.Core.Submissions;
using Kysect.Shreks.Seeding.Options;
using Microsoft.Extensions.DependencyInjection;

namespace Kysect.Shreks.Tests.Handlers.GithubEvents;

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