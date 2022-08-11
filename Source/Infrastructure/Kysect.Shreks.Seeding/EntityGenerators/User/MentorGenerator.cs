using Bogus;
using Kysect.Shreks.Core.Users;
using Kysect.Shreks.Seeding.Options;

namespace Kysect.Shreks.Seeding.EntityGenerators;

public class MentorGenerator : EntityGeneratorBase<Mentor>
{
    private readonly Faker _faker;
    
    public MentorGenerator(EntityGeneratorOptions<Mentor> options, Faker faker) : base(options)
    {
        _faker = faker;
    }

    protected override Mentor Generate(int index)
    {
        var middleName = _faker.Random.Bool(0.95f)
            ? _faker.Name.FirstName()
            : string.Empty;

        return new Mentor
        (
            _faker.Name.FirstName(), 
            middleName, 
            _faker.Name.LastName()
        );
    }
}