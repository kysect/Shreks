using Bogus;
using ITMO.Dev.ASAP.Core.Study;
using ITMO.Dev.ASAP.Seeding.Options;

namespace ITMO.Dev.ASAP.Seeding.EntityGenerators;

public class SubjectGenerator : EntityGeneratorBase<Subject>
{
    private readonly Faker _faker;

    public SubjectGenerator(EntityGeneratorOptions<Subject> options, Faker faker)
        : base(options)
    {
        _faker = faker;
    }

    protected override Subject Generate(int index)
    {
        return new Subject(_faker.Random.Guid(), _faker.Commerce.Product());
    }
}