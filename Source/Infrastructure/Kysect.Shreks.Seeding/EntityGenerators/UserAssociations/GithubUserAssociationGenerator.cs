using Bogus;
using Kysect.Shreks.Core.UserAssociations;
using Kysect.Shreks.Core.Users;
using Kysect.Shreks.Seeding.Options;

namespace Kysect.Shreks.Seeding.EntityGenerators;

public class GithubUserAssociationGenerator : EntityGeneratorBase<GithubUserAssociation>
{
    private readonly IEntityGenerator<User> _userGenerator;

    private readonly Faker _faker;

    public GithubUserAssociationGenerator(
        EntityGeneratorOptions<GithubUserAssociation> options,
        Faker faker,
        IEntityGenerator<User> userGenerator) : base(options)
    {
        _faker = faker;
        _userGenerator = userGenerator;
    }

    protected override GithubUserAssociation Generate(int index)
    {
        if (index >= _userGenerator.GeneratedEntities.Count)
            throw new IndexOutOfRangeException("Github association count is greater than count of users.");

        var user = _userGenerator.GeneratedEntities[index];

        var association = new GithubUserAssociation(user, _faker.Random.Guid().ToString());

        return association;
    }
}