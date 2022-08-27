using Bogus;
using Kysect.Shreks.Core.DeadlinePolicies;
using Kysect.Shreks.Seeding.Extensions;
using Kysect.Shreks.Seeding.Options;

namespace Kysect.Shreks.Seeding.EntityGenerators;

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
            // TODO: remove limit after WI-229
            0 => new AbsoluteDeadlinePolicy(_faker.Date.Timespan(TimeSpan.FromHours(12)), _faker.Random.Points(0, 10)),
            1 => new FractionDeadlinePolicy(_faker.Date.Timespan(TimeSpan.FromHours(12)), _faker.Random.Double()),
            2 => new CappingDeadlinePolicy(_faker.Date.Timespan(TimeSpan.FromHours(12)), _faker.Random.Double(0, 5)),
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}