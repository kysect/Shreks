using Bogus;
using Kysect.Shreks.Core.Study;
using Kysect.Shreks.Core.SubjectCourseAssociations;
using Kysect.Shreks.Seeding.Options;

namespace Kysect.Shreks.Seeding.EntityGenerators;

public class SubjectCourseAssociationGenerator : EntityGeneratorBase<GithubSubjectCourseAssociation>
{
    private readonly IEntityGenerator<SubjectCourse> _subjectCourseGenerator;
    private readonly Faker _faker;

    public SubjectCourseAssociationGenerator(
        EntityGeneratorOptions<GithubSubjectCourseAssociation> options,
        IEntityGenerator<SubjectCourse> subjectCourseGenerator,
        Faker faker)
        : base(options)
    {
        _subjectCourseGenerator = subjectCourseGenerator;
        _faker = faker;
    }

    protected override GithubSubjectCourseAssociation Generate(int index)
    {
        var count = _subjectCourseGenerator.GeneratedEntities.Count;

        if (index >= count)
        {
            string message = "GitHub subject course association index more than count of subject courses.";
            throw new IndexOutOfRangeException(message);
        }

        var subjectCourse = _subjectCourseGenerator.GeneratedEntities[index];

        var association = new GithubSubjectCourseAssociation(
            subjectCourse,
            _faker.Random.Guid().ToString(),
            _faker.Internet.UserName());

        subjectCourse.AddAssociation(association);

        return association;
    }
}