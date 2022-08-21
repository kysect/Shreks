using Kysect.Shreks.Core.Study;
using Kysect.Shreks.Core.SubjectCourseAssociations;
using Kysect.Shreks.Core.UserAssociations;
using Kysect.Shreks.Core.Users;
using Kysect.Shreks.DataAccess.Abstractions;
using Kysect.Shreks.DataAccess.Extensions;
using Kysect.Shreks.Seeding.EntityGenerators;
using Kysect.Shreks.Seeding.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Kysect.Shreks.Playground.Github.TestEnv;

public static class TestEnv
{
    public static IServiceCollection AddGithubPlaygroundDatabase(this IServiceCollection serviceCollection, TestEnvConfiguration config)
    {
        serviceCollection
            .AddDatabaseContext(optionsBuilder => optionsBuilder
                .UseSqlite("Filename=shreks-gh.db")
                .UseLazyLoadingProxies());

        serviceCollection.AddEntityGenerators(o =>
        {
            o.ConfigureFaker(o => o.Locale = "ru");
            o.ConfigureEntityGenerator<User>(o => o.Count = config.Users.Count);
            o.ConfigureEntityGenerator<Student>(o => o.Count = config.Users.Count);
            o.ConfigureEntityGenerator<GithubUserAssociation>(o => o.Count = 0);
            o.ConfigureEntityGenerator<IsuUserAssociation>(o => o.Count = 0);
            o.ConfigureEntityGenerator<Submission>(o => o.Count = 0);
            o.ConfigureEntityGenerator<SubjectCourse>(o => o.Count = 1);
            o.ConfigureEntityGenerator<SubjectCourseAssociation>(o => o.Count = 0);
        });

        return serviceCollection.AddDatabaseSeeders();
    }

    public static async Task UseTestEnv(
        this IServiceProvider serviceProvider, 
        TestEnvConfiguration config,
        CancellationToken cancellationToken = default)
    {
        await serviceProvider.UseDatabaseSeeders(cancellationToken);

        using var scope  = serviceProvider.CreateScope();
        
        var dbContext = scope.ServiceProvider.GetRequiredService<IShreksDatabaseContext>();

        var userGenerator = serviceProvider.GetRequiredService<IEntityGenerator<User>>();
        var users = userGenerator.GeneratedEntities;
        dbContext.Users.AttachRange(users);
        for (var index = 0; index < users.Count; index++)
        {
            var user = users[index];
            var login = config.Users[index];
            dbContext.UserAssociations.Add(new GithubUserAssociation(user, login));
        }
        
        var subjectCourseGenerator = serviceProvider.GetRequiredService<IEntityGenerator<SubjectCourse>>();
        var subjectCourse = subjectCourseGenerator.GeneratedEntities[0];
        dbContext.SubjectCourses.Attach(subjectCourse);
        dbContext.SubjectCourseAssociations.Add(
            new GithubSubjectCourseAssociation(subjectCourse, config.Organization));

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}