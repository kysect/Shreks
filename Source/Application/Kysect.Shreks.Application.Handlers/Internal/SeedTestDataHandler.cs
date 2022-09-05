using Kysect.Shreks.Core.Study;
using Kysect.Shreks.Core.SubjectCourseAssociations;
using Kysect.Shreks.Core.UserAssociations;
using Kysect.Shreks.Core.Users;
using Kysect.Shreks.DataAccess.Abstractions;
using Kysect.Shreks.Playground.Github.TestEnv;
using Kysect.Shreks.Seeding.EntityGenerators;
using MediatR;
using static Kysect.Shreks.Application.Abstractions.Internal.SeedTestData;

namespace Kysect.Shreks.Application.Handlers.Internal;

public class SeedTestDataHandler : IRequestHandler<Query>
{
    private const string ExceptedEnvironment = "Testing";

    private readonly IShreksDatabaseContext _context;
    private readonly IEntityGenerator<User> _userGenerator;
    private readonly IEntityGenerator<SubjectCourse> _subjectCourseGenerator;
    private readonly TestEnvironmentConfiguration _configuration;

    public SeedTestDataHandler(
        IShreksDatabaseContext context,
        IEntityGenerator<User> userGenerator,
        IEntityGenerator<SubjectCourse> subjectCourseGenerator,
        TestEnvironmentConfiguration configuration)
    {
        _context = context;
        _userGenerator = userGenerator;
        _subjectCourseGenerator = subjectCourseGenerator;
        _configuration = configuration;
    }

    public async Task<Unit> Handle(Query request, CancellationToken cancellationToken = default)
    {
        EnsureUserAcknowledgedEnvironment(request);
        AddUsers();
        AddGithubUserAssociations();

        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }

    private static void EnsureUserAcknowledgedEnvironment(Query request)
    {
        if (!request.Environment.Equals(ExceptedEnvironment, StringComparison.OrdinalIgnoreCase))
        {
            throw new UserNotAcknowledgedEnvironmentException();
        }
    }

    private void AddGithubUserAssociations()
    {
        SubjectCourse subjectCourse = _subjectCourseGenerator.GeneratedEntities[0];
        _context.SubjectCourses.Attach(subjectCourse);
        _context.SubjectCourseAssociations.Add(
            new GithubSubjectCourseAssociation(subjectCourse, _configuration.Organization,
                _configuration.TemplateRepository));
    }

    private void AddUsers()
    {
        IReadOnlyList<User> users = _userGenerator.GeneratedEntities;
        _context.Users.AttachRange(users);

        for (var index = 0; index < _configuration.Users.Count; index++)
        {
            User user = users[index];
            string login = _configuration.Users[index];
            _context.UserAssociations.Add(new GithubUserAssociation(user, login));
        }
    }
}