using Bogus;
using Kysect.Shreks.Core.Study;
using Kysect.Shreks.Core.SubjectCourseAssociations;
using Kysect.Shreks.Core.SubmissionStateWorkflows;
using Kysect.Shreks.Core.Users;
using Kysect.Shreks.Core.ValueObject;
using Kysect.Shreks.DataAccess.Abstractions;
using Kysect.Shreks.Seeding.EntityGenerators;

namespace Kysect.Shreks.Tests.GithubWorkflow.Tools;

public class GithubApplicationTestContextGenerator
{
    private readonly IShreksDatabaseContext _context;
    private readonly Faker _faker;
    private readonly IEntityGenerator<StudentGroup> _studentGroupGenerator;
    private readonly IEntityGenerator<Subject> _subjectGenerator;
    private readonly IEntityGenerator<User> _userGenerator;

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

        _context.Students.Add(student);
        _context.StudentGroups.Add(group);

        Subject subject = _subjectGenerator.Generate();
        var subjectCourse =
            new SubjectCourse(subject, _faker.Commerce.ProductName(), SubmissionStateWorkflowType.ReviewOnly);
        var githubSubjectCourseAssociation = new GithubSubjectCourseAssociation(subjectCourse,
            _faker.Company.CompanyName(), _faker.Commerce.ProductName());
        var subjectCourseGroup = new SubjectCourseGroup(subjectCourse, group);
        var assignment =
            new Assignment(_faker.Hacker.Verb(), "task-0", 1, new Points(0), new Points(10), subjectCourse);
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