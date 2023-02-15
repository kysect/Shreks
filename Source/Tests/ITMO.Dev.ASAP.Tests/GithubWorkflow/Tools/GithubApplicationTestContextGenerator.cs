using Bogus;
using ITMO.Dev.ASAP.Core.Study;
using ITMO.Dev.ASAP.Core.SubjectCourseAssociations;
using ITMO.Dev.ASAP.Core.SubmissionStateWorkflows;
using ITMO.Dev.ASAP.Core.Users;
using ITMO.Dev.ASAP.Core.ValueObject;
using ITMO.Dev.ASAP.DataAccess.Abstractions;
using ITMO.Dev.ASAP.Seeding.EntityGenerators;

namespace ITMO.Dev.ASAP.Tests.GithubWorkflow.Tools;

public class GithubApplicationTestContextGenerator
{
    private readonly IDatabaseContext _context;
    private readonly Faker _faker;
    private readonly IEntityGenerator<StudentGroup> _studentGroupGenerator;
    private readonly IEntityGenerator<Subject> _subjectGenerator;
    private readonly IEntityGenerator<User> _userGenerator;

    public GithubApplicationTestContextGenerator(
        Faker faker,
        IDatabaseContext context,
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

        _context.Students.Add(student);
        _context.StudentGroups.Add(group);

        Subject subject = _subjectGenerator.Generate();
        var subjectCourse = new SubjectCourse(
            _faker.Random.Guid(),
            subject,
            _faker.Commerce.ProductName(),
            SubmissionStateWorkflowType.ReviewOnly);

        var githubSubjectCourseAssociation = new GithubSubjectCourseAssociation(
            _faker.Random.Guid(),
            subjectCourse,
            _faker.Company.CompanyName(),
            _faker.Commerce.ProductName(),
            _faker.Commerce.ProductName());

        var subjectCourseGroup = new SubjectCourseGroup(subjectCourse, group);

        var assignment = new Assignment(
            _faker.Random.Guid(),
            _faker.Hacker.Verb(),
            "task-0",
            1,
            new Points(0),
            new Points(10),
            subjectCourse);

        subjectCourse.AddAssignment(assignment);
        var groupAssignment = new GroupAssignment(group, assignment, DateOnly.FromDateTime(DateTime.Now));

        _context.SubjectCourses.Add(subjectCourse);
        _context.SubjectCourseAssociations.Add(githubSubjectCourseAssociation);
        _context.SubjectCourseGroups.Add(subjectCourseGroup);
        _context.Assignments.Add(assignment);
        _context.GroupAssignments.Add(groupAssignment);

        await _context.SaveChangesAsync(CancellationToken.None);

        return new GithubApplicationTestContext(githubSubjectCourseAssociation, student);
    }
}