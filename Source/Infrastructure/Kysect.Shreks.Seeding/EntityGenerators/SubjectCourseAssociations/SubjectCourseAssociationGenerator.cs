using Bogus;
using Kysect.Shreks.Core.Study;
using Kysect.Shreks.Core.SubjectCourseAssociations;
using Kysect.Shreks.Seeding.Options;

namespace Kysect.Shreks.Seeding.EntityGenerators;

public class SubjectCourseAssociationGenerator : EntityGeneratorBase<SubjectCourseAssociation>
{
    private readonly IEntityGenerator<SubjectCourse> _subjectCourseGenerator;
    private readonly Faker _faker;

    public SubjectCourseAssociationGenerator(
        EntityGeneratorOptions<SubjectCourseAssociation> options,
        IEntityGenerator<SubjectCourse> subjectCourseGenerator,
        Faker faker)
        : base(options)
    {
        _subjectCourseGenerator = subjectCourseGenerator;
        _faker = faker;
    }

    protected override SubjectCourseAssociation Generate(int index)
    {
        var count = _subjectCourseGenerator.GeneratedEntities.Count;

        if (index >= count)
            throw new IndexOutOfRangeException("Subject course index more than count of subject courses.");

        var subjectCourse = _subjectCourseGenerator.GeneratedEntities[index];

        var association = new GithubSubjectCourseAssociation(subjectCourse, _faker.Internet.UserName());
        subjectCourse.AddAssociation(association);

        return association;
    }
}