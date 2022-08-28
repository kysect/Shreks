using Bogus;
using Kysect.Shreks.Core.Study;
using Kysect.Shreks.Core.Users;
using Kysect.Shreks.Seeding.Options;

namespace Kysect.Shreks.Seeding.EntityGenerators;

public class SubjectCourseGenerator : EntityGeneratorBase<SubjectCourse>
{
    private readonly Faker _faker;
    private readonly IEntityGenerator<User> _userGenerator;
    private readonly IEntityGenerator<Subject> _subjectGenerator;

    public SubjectCourseGenerator(
        EntityGeneratorOptions<SubjectCourse> options,
        IEntityGenerator<User> userGenerator,
        IEntityGenerator<Subject> subjectGenerator,
        Faker faker) 
        : base(options)
    {
        _userGenerator = userGenerator;
        _subjectGenerator = subjectGenerator;
        _faker = faker;
    }

    protected override SubjectCourse Generate(int index)
    {
        var subjectCount = _subjectGenerator.GeneratedEntities.Count;

        if (index >= subjectCount)
            throw new IndexOutOfRangeException("The subject index is greater than the number of subjects.");

        var subject = _subjectGenerator.GeneratedEntities[index];

        var subjectCourseName = _faker.Commerce.ProductName();

        var subjectCourse = new SubjectCourse(subject, subjectCourseName);

        IEnumerable<User> users = _faker.Random
            .ListItems(_userGenerator.GeneratedEntities.ToList())
            .Distinct();

        foreach (var user in users)
        {
            subjectCourse.AddMentor(user);
        }

        subject.AddCourse(subjectCourse);

        return subjectCourse;
    }
}