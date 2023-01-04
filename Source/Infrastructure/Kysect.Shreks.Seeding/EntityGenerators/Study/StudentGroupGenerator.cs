using Bogus;
using Kysect.Shreks.Core.Study;
using Kysect.Shreks.Seeding.Options;

namespace Kysect.Shreks.Seeding.EntityGenerators;

public class StudentGroupGenerator : EntityGeneratorBase<StudentGroup>
{
    private const int MinGroupNumber = 10000;
    private const int MaxGroupNumber = 100000;

    private readonly Faker _faker;

    public StudentGroupGenerator(EntityGeneratorOptions<StudentGroup> options, Faker faker)
        : base(options)
    {
        _faker = faker;
    }

    protected override StudentGroup Generate(int index)
    {
        int groupNumber = _faker.Random.Int(MinGroupNumber, MaxGroupNumber);
        return new StudentGroup($"M{groupNumber}");
    }
}