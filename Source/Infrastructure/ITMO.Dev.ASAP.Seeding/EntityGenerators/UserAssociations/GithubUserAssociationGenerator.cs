using Bogus;
using ITMO.Dev.ASAP.Core.UserAssociations;
using ITMO.Dev.ASAP.Core.Users;
using ITMO.Dev.ASAP.Seeding.Options;

namespace ITMO.Dev.ASAP.Seeding.EntityGenerators;

public class GithubUserAssociationGenerator : EntityGeneratorBase<GithubUserAssociation>
{
    private readonly Faker _faker;
    private readonly IEntityGenerator<User> _userGenerator;

    public GithubUserAssociationGenerator(
        EntityGeneratorOptions<GithubUserAssociation> options,
        Faker faker,
        IEntityGenerator<User> userGenerator)
        : base(options)
    {
        _faker = faker;
        _userGenerator = userGenerator;
    }

    protected override GithubUserAssociation Generate(int index)
    {
        if (index >= _userGenerator.GeneratedEntities.Count)
            throw new IndexOutOfRangeException("Github association count is greater than count of users.");

        User user = _userGenerator.GeneratedEntities[index];

        var association = new GithubUserAssociation(_faker.Random.Guid(), user, _faker.Random.Guid().ToString());

        return association;
    }
}