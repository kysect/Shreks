using Bogus;
using Kysect.Shreks.Core.Users;
using Kysect.Shreks.Seeding.Options;

namespace Kysect.Shreks.Seeding.EntityGenerators.UserGenerators;

public class MentorGenerator : EntityGeneratorBase<Mentor>
{
    private readonly Faker _faker;
    
    public MentorGenerator(EntityGeneratorOptions<Mentor> options, Faker faker) : base(options)
    {
        _faker = faker;
    }

    protected override Mentor Generate(int index)
    {
        return new Mentor
        (
            _faker.Person.FirstName, 
            _faker.Person.UserName, 
            _faker.Person.LastName
        );
    }
}