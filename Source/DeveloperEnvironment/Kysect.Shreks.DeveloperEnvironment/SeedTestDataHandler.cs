using Kysect.Shreks.Core.Study;
using Kysect.Shreks.Core.SubjectCourseAssociations;
using Kysect.Shreks.Core.UserAssociations;
using Kysect.Shreks.Core.Users;
using Kysect.Shreks.DataAccess.Abstractions;
using Kysect.Shreks.Seeding.EntityGenerators;

namespace Kysect.Shreks.DeveloperEnvironment;

public class DeveloperEnvironmentSeeder
{
    private const string ExceptedEnvironment = "Testing";

    private readonly IShreksDatabaseContext _context;
    private readonly IEntityGenerator<User> _userGenerator;
    private readonly IEntityGenerator<SubjectCourse> _subjectCourseGenerator;

    public DeveloperEnvironmentSeeder(
        IShreksDatabaseContext context,
        IEntityGenerator<User> userGenerator,
        IEntityGenerator<SubjectCourse> subjectCourseGenerator)
    {
        _context = context;
        _userGenerator = userGenerator;
        _subjectCourseGenerator = subjectCourseGenerator;
    }

    public async Task Handle(DeveloperEnvironmentSeedingRequest request, CancellationToken cancellationToken = default)
    {
        EnsureUserAcknowledgedEnvironment(request);
        AddUsers(request);
        AddGithubUserAssociations(request);

        await _context.SaveChangesAsync(cancellationToken);
    }

    private static void EnsureUserAcknowledgedEnvironment(DeveloperEnvironmentSeedingRequest request)
    {
        if (!request.Environment.Equals(ExceptedEnvironment, StringComparison.OrdinalIgnoreCase))
        {
            throw new UserNotAcknowledgedEnvironmentException();
        }
    }

    private void AddGithubUserAssociations(DeveloperEnvironmentSeedingRequest request)
    {
        SubjectCourse subjectCourse = _subjectCourseGenerator.GeneratedEntities[0];
        _context.SubjectCourses.Attach(subjectCourse);
        var githubSubjectCourseAssociation = new GithubSubjectCourseAssociation(subjectCourse, request.Organization, request.TemplateRepository);
        _context.SubjectCourseAssociations.Add(githubSubjectCourseAssociation);
    }

    private void AddUsers(DeveloperEnvironmentSeedingRequest request)
    {
        IReadOnlyList<User> users = _userGenerator.GeneratedEntities;
        _context.Users.AttachRange(users);

        for (int index = 0; index < request.Users.Count; index++)
        {
            User user = users[index];
            string login = request.Users[index];
            _context.UserAssociations.Add(new GithubUserAssociation(user, login));
        }
    }
}