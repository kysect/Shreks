using Bogus;
using Kysect.Shreks.Core.DeadlinePolicies;
using Kysect.Shreks.Core.Study;
using Kysect.Shreks.Core.SubmissionStateWorkflows;
using Kysect.Shreks.Core.Users;
using Kysect.Shreks.Seeding.Options;

namespace Kysect.Shreks.Seeding.EntityGenerators;

public class SubjectCourseGenerator : EntityGeneratorBase<SubjectCourse>
{
    private readonly Faker _faker;
    private readonly IEntityGenerator<User> _userGenerator;
    private readonly IEntityGenerator<Subject> _subjectGenerator;
    private readonly IEntityGenerator<DeadlinePolicy> _deadlinePolicyGenerator;

    public SubjectCourseGenerator(
        EntityGeneratorOptions<SubjectCourse> options,
        IEntityGenerator<User> userGenerator,
        IEntityGenerator<Subject> subjectGenerator,
        Faker faker,
        IEntityGenerator<DeadlinePolicy> deadlinePolicyGenerator)
        : base(options)
    {
        _userGenerator = userGenerator;
        _subjectGenerator = subjectGenerator;
        _faker = faker;
        _deadlinePolicyGenerator = deadlinePolicyGenerator;
    }

    protected override SubjectCourse Generate(int index)
    {
        var subjectCount = _subjectGenerator.GeneratedEntities.Count;

        var deadlineCount = _faker.Random.Int(0, _deadlinePolicyGenerator.GeneratedEntities.Count);

        IEnumerable<DeadlinePolicy> deadlines = Enumerable.Range(0, deadlineCount)
            .Select(_ => _faker.Random.Int(0, _deadlinePolicyGenerator.GeneratedEntities.Count - 1))
            .Select(i => _deadlinePolicyGenerator.GeneratedEntities[i])
            .Distinct();

        if (index >= subjectCount)
            throw new IndexOutOfRangeException("The subject index is greater than the number of subjects.");

        var subject = _subjectGenerator.GeneratedEntities[index];

        var subjectCourseName = _faker.Commerce.ProductName();

        SubmissionStateWorkflowType reviewType = SubmissionStateWorkflowType.ReviewWithDefense;

        var subjectCourse = new SubjectCourse(subject, subjectCourseName, reviewType);

        IEnumerable<User> users = _faker.Random
            .ListItems(_userGenerator.GeneratedEntities.ToList(), 2)
            .Distinct();

        foreach (var user in users)
        {
            subjectCourse.AddMentor(user);
        }

        foreach (var deadline in deadlines)
        {
            subjectCourse.AddDeadlinePolicy(deadline);
        }

        subject.AddCourse(subjectCourse);

        return subjectCourse;
    }
}