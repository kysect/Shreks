using Bogus;
using Kysect.Shreks.Core.Study;
using Kysect.Shreks.Core.SubjectCourseAssociations;
using Kysect.Shreks.Seeding.Options;

namespace Kysect.Shreks.Seeding.EntityGenerators;

public class SubjectCourseAssociationGenerator : EntityGeneratorBase<GithubSubjectCourseAssociation>
{
    private readonly Faker _faker;
    private readonly IEntityGenerator<SubjectCourse> _subjectCourseGenerator;

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
        int count = _subjectCourseGenerator.GeneratedEntities.Count;

        if (index >= count)
        {
            const string message = "GitHub subject course association index more than count of subject courses.";
            throw new ArgumentOutOfRangeException(nameof(index), message);
        }

        SubjectCourse subjectCourse = _subjectCourseGenerator.GeneratedEntities[index];

        var association = new GithubSubjectCourseAssociation(
            subjectCourse,
            _faker.Random.Guid().ToString(),
            _faker.Internet.UserName());

        subjectCourse.AddAssociation(association);

        return association;
    }
}