using ITMO.Dev.ASAP.Application.GithubWorkflow.Abstractions.Models;
using ITMO.Dev.ASAP.Core.Study;
using ITMO.Dev.ASAP.Core.SubjectCourseAssociations;
using ITMO.Dev.ASAP.Core.Submissions;
using ITMO.Dev.ASAP.Core.UserAssociations;
using ITMO.Dev.ASAP.Core.Users;
using ITMO.Dev.ASAP.Seeding.Extensions;

namespace ITMO.Dev.ASAP.WebApi.Extensions;

internal static class TestEnvironmentExtensions
{
    internal static void AddEntityGeneratorsAndSeeding(
        this IServiceCollection serviceCollection,
        TestEnvironmentConfiguration testEnvironmentConfiguration)
    {
        serviceCollection.AddEntityGenerators(o =>
            {
                o.ConfigureFaker(o => o.Locale = "ru");
                o.ConfigureEntityGenerator<User>(o => o.Count = testEnvironmentConfiguration.Users.Count);
                o.ConfigureEntityGenerator<Student>(o => o.Count = testEnvironmentConfiguration.Users.Count);
                o.ConfigureEntityGenerator<Mentor>(o => o.Count = testEnvironmentConfiguration.Users.Count);
                o.ConfigureEntityGenerator<GithubUserAssociation>(o => o.Count = 0);
                o.ConfigureEntityGenerator<IsuUserAssociation>(o => o.Count = 0);
                o.ConfigureEntityGenerator<Submission>(o => o.Count = 0);
                o.ConfigureEntityGenerator<SubjectCourse>(o => o.Count = 1);
                o.ConfigureEntityGenerator<SubjectCourseAssociation>(o => o.Count = 0);
            })
            .AddDatabaseSeeders();
    }
}