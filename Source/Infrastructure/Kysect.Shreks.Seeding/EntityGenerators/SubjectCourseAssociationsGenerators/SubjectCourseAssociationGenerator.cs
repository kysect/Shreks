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
        var number = _faker.Random.Number(0, count - 1);
        var subjectCourse = _subjectCourseGenerator.GeneratedEntities[number];
        
        var association = new GithubSubjectCourseAssociation(subjectCourse, _faker.Commerce.Product());
        subjectCourse.AddAssociation(association);

        return association;
    }
}