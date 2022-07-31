using Bogus;
using Kysect.Shreks.Core.DeadlinePolicies;
using Kysect.Shreks.Seeding.Options;

namespace Kysect.Shreks.Seeding.EntityGenerators.DeadlinePolicyGenerator;

public class DeadlinePolicyGenerator : EntityGeneratorBase<DeadlinePolicy>
{
    private readonly Faker _faker;

    public DeadlinePolicyGenerator(EntityGeneratorOptions<DeadlinePolicy> options, Faker faker) : base(options)
    {
        _faker = faker;
    }

    protected override DeadlinePolicy Generate(int index)
    {
        return (index % 3) switch
        {
            0 => new AbsoluteDeadlinePolicy(_faker.Date.Timespan(), _faker.Random.Double(0, 10)),
            1 => new FractionDeadlinePolicy(_faker.Date.Timespan(), _faker.Random.Double()),
            2 => new CappingDeadlinePolicy(_faker.Date.Timespan(), _faker.Random.Double(0, 5)),
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}