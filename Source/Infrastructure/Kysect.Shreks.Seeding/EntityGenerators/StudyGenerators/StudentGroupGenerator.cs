using Bogus;
using Kysect.Shreks.Core.Study;
using Kysect.Shreks.Seeding.Options;

namespace Kysect.Shreks.Seeding.EntityGenerators.StudyGenerators;

public class StudentGroupGenerator : EntityGeneratorBase<StudentGroup>
{
    private Faker _faker;
    
    public StudentGroupGenerator(EntityGeneratorOptions<StudentGroup> options, Faker faker) : base(options)
    {
        _faker = faker;
    }

    protected override StudentGroup Generate(int index)
    {
        return new StudentGroup(_faker.Commerce.Product());
    }
}