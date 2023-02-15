using Bogus;
using ITMO.Dev.ASAP.Core.Study;
using ITMO.Dev.ASAP.Core.SubjectCourseAssociations;
using ITMO.Dev.ASAP.Seeding.Options;

namespace ITMO.Dev.ASAP.Seeding.EntityGenerators;

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
            _faker.Random.Guid(),
            subjectCourse,
            _faker.Random.Guid().ToString(),
            _faker.Internet.UserName(),
            _faker.Internet.UserAgent());

        return association;
    }
}