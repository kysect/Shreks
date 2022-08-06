using Bogus;
using Kysect.Shreks.Core.Study;
using Kysect.Shreks.Seeding.Options;

namespace Kysect.Shreks.Seeding.EntityGenerators;

public class SubjectGenerator : EntityGeneratorBase<Subject>
{
    private Faker _faker;
    
    public SubjectGenerator(EntityGeneratorOptions<Subject> options, Faker faker) : base(options)
    {
        _faker = faker;
    }

    protected override Subject Generate(int index)
    {
        return new Subject(_faker.Commerce.Product());
    }
}