using Bogus;
using Kysect.Shreks.Core.Study;
using Kysect.Shreks.Core.SubjectCourseAssociations;
using Kysect.Shreks.Core.SubmissionStateWorkflows;
using Kysect.Shreks.Core.Users;
using Kysect.Shreks.DataAccess.Abstractions;
using Kysect.Shreks.Seeding.EntityGenerators;

namespace Kysect.Shreks.Tests.GithubWorkflow.Tools;

public class GithubApplicationTestContextGenerator
{
    private readonly Faker _faker;
    private readonly IShreksDatabaseContext _context;
    private readonly IEntityGenerator<User> _userGenerator;
    private readonly IEntityGenerator<StudentGroup> _studentGroupGenerator;
    private readonly IEntityGenerator<Subject> _subjectGenerator;

    public GithubApplicationTestContextGenerator(
        Faker faker,
        IShreksDatabaseContext context,
        IEntityGenerator<User> userGenerator,
        IEntityGenerator<StudentGroup> studentGroupGenerator,
        IEntityGenerator<Subject> subjectGenerator)
    {
        _context = context;
        _userGenerator = userGenerator;
        _studentGroupGenerator = studentGroupGenerator;
        _subjectGenerator = subjectGenerator;
        _faker = faker;
    }

    public async Task<GithubApplicationTestContext> Create()
    {
        string userAssociation = _faker.Internet.UserName();

        StudentGroup group = _studentGroupGenerator.Generate();
        User user = _userGenerator.Generate();
        var student = new Student(user, group);
        student.AddGithubAssociation(userAssociation);
        group.AddStudent(student);

        _context.Students.Add(student);
        _context.StudentGroups.Add(group);

        Subject subject = _subjectGenerator.Generate();
        var subjectCourse = new SubjectCourse(subject, _faker.Commerce.ProductName(), SubmissionStateWorkflowType.ReviewOnly);
        var githubSubjectCourseAssociation = new GithubSubjectCourseAssociation(subjectCourse, _faker.Company.CompanyName(), _faker.Commerce.ProductName());
        var subjectCourseGroup = new SubjectCourseGroup(subjectCourse, group);

        _context.SubjectCourses.Add(subjectCourse);
        _context.SubjectCourseAssociations.Add(githubSubjectCourseAssociation);
        _context.SubjectCourseGroups.Add(subjectCourseGroup);

        await _context.SaveChangesAsync(CancellationToken.None);

        return new GithubApplicationTestContext(githubSubjectCourseAssociation, student);
    }
}