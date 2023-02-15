using Bogus;
using ITMO.Dev.ASAP.Core.UserAssociations;
using ITMO.Dev.ASAP.Core.Users;
using ITMO.Dev.ASAP.Seeding.Options;

namespace ITMO.Dev.ASAP.Seeding.EntityGenerators;

public class IsuUserAssociationGenerator : EntityGeneratorBase<IsuUserAssociation>
{
    private const int MinIsuNumber = 100000;
    private const int MaxIsuNumber = 1000000;

    private readonly Faker _faker;

    private readonly IEntityGenerator<User> _userGenerator;

    public IsuUserAssociationGenerator(
        EntityGeneratorOptions<IsuUserAssociation> options,
        Faker faker,
        IEntityGenerator<User> userGenerator)
        : base(options)
    {
        _faker = faker;
        _userGenerator = userGenerator;
    }

    protected override IsuUserAssociation Generate(int index)
    {
        if (index >= _userGenerator.GeneratedEntities.Count)
        {
            const string message = "Isu association count is greater than count of users.";
            throw new ArgumentOutOfRangeException(nameof(index), message);
        }

        User user = _userGenerator.GeneratedEntities[index];

        foreach (UserAssociation userAssociation in user.Associations)
        {
            if (userAssociation is IsuUserAssociation isuUserAssociation)
                return isuUserAssociation;
        }

        int id = _faker.Random.Int(MinIsuNumber, MaxIsuNumber);
        var association = new IsuUserAssociation(_faker.Random.Guid(), user, id);

        return association;
    }
}