using Bogus;
using Kysect.Shreks.Core.UserAssociations;
using Kysect.Shreks.Core.Users;
using Kysect.Shreks.Seeding.Options;

namespace Kysect.Shreks.Seeding.EntityGenerators;

public class IsuUserAssociationGenerator : EntityGeneratorBase<IsuUserAssociation>
{
    private readonly IEntityGenerator<User> _userGenerator;
    private readonly Faker _faker;
    
    public IsuUserAssociationGenerator(
        EntityGeneratorOptions<IsuUserAssociation> options,
        IEntityGenerator<User> userGenerator,
        Faker faker) : base(options)
    {
        _userGenerator = userGenerator;
        _faker = faker;
    }

    protected override IsuUserAssociation Generate(int index)
    {
        var userCount = _userGenerator.GeneratedEntities.Count;
        
        if (index >= userCount)
            throw new IndexOutOfRangeException("User index more than count of users.");

        var user = _userGenerator.GeneratedEntities[index];

        var id = _faker.Random.Int();
        var association = new IsuUserAssociation(user, id);

        user.AddAssociation(association);

        return association;
    }
}