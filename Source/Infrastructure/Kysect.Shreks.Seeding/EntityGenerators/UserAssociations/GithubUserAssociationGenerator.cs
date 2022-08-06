using Bogus;
using Kysect.Shreks.Core.UserAssociations;
using Kysect.Shreks.Core.Users;
using Kysect.Shreks.Seeding.Options;

namespace Kysect.Shreks.Seeding.EntityGenerators;

public class GithubUserAssociationGenerator : EntityGeneratorBase<GithubUserAssociation>
{
    private readonly IEntityGenerator<User> _userGenerator;
    private readonly Faker _faker;
    
    public GithubUserAssociationGenerator(EntityGeneratorOptions<GithubUserAssociation> options, IEntityGenerator<User> userGenerator, Faker faker) : base(options)
    {
        _userGenerator = userGenerator;
        _faker = faker;
    }

    protected override GithubUserAssociation Generate(int index)
    {
        var userCount = _userGenerator.GeneratedEntities.Count;
        var userNumber = _faker.Random.Number(0, userCount - 1);

        var user = _userGenerator.GeneratedEntities[userNumber];

        var githubName = _faker.Person.UserName;
        var association = new GithubUserAssociation(user, githubName);

        user.AddAssociation(association);

        return association;
    }
}
