using Bogus;
using Kysect.Shreks.Core.Users;
using Kysect.Shreks.Seeding.Options;

namespace Kysect.Shreks.Seeding.EntityGenerators;

public class MentorGenerator : EntityGeneratorBase<Mentor>
{
    private readonly Faker _faker;
    private readonly IEntityGenerator<User> _userGenerator;

    public MentorGenerator(
        EntityGeneratorOptions<Mentor> options,
        Faker faker,
        IEntityGenerator<User> userGenerator) : base(options)
    {
        _faker = faker;
        _userGenerator = userGenerator;
    }

    protected override Mentor Generate(int index)
    {
        var userIndex = _faker.Random.Int(0, _userGenerator.GeneratedEntities.Count - 1);
        var user = _userGenerator.GeneratedEntities[userIndex];
        
        return new Mentor(user);
    }
}