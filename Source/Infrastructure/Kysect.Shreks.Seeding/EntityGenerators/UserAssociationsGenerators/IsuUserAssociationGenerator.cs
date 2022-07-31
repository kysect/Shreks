using Bogus;
using Kysect.Shreks.Core.UserAssociations;
using Kysect.Shreks.Core.Users;
using Kysect.Shreks.Seeding.Options;

namespace Kysect.Shreks.Seeding.EntityGenerators.UserAssociationsGenerators;

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
        var userNumber = _faker.Random.Number(0, userCount - 1);

        var user = _userGenerator.GeneratedEntities[userNumber];

        var id = _faker.Random.Int();
        var association = new IsuUserAssociation(user, id);

        user.AddAssociation(association);

        return association;
    }
}