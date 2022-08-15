using Bogus;
using Kysect.Shreks.Core.UserAssociations;
using Kysect.Shreks.Core.Users;
using Kysect.Shreks.Seeding.Options;

namespace Kysect.Shreks.Seeding.EntityGenerators;

public class IsuUserAssociationGenerator : EntityGeneratorBase<IsuUserAssociation>
{
    private const int MinIsuNumber = 100000;
    private const int MaxIsuNumber = 1000000;

    private readonly IEntityGenerator<User> _userGenerator;

    private readonly Faker _faker;

    public IsuUserAssociationGenerator(
        EntityGeneratorOptions<IsuUserAssociation> options,
        Faker faker,
        IEntityGenerator<User> userGenerator) : base(options)
    {
        _faker = faker;
        _userGenerator = userGenerator;
    }

    protected override IsuUserAssociation Generate(int index)
    {
        if (index >= _userGenerator.GeneratedEntities.Count)
            throw new IndexOutOfRangeException("Isu association count is greater than count of users.");
        
        var user = _userGenerator.GeneratedEntities[index];

        var id = _faker.Random.Int(MinIsuNumber, MaxIsuNumber);
        var association = new IsuUserAssociation(user, id);

        user.AddAssociation(association);

        return association;
    }
}