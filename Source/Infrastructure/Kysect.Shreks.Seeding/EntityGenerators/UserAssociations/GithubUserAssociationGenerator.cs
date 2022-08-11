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

        if (index >= userCount)
            throw new IndexOutOfRangeException("User index more than count of users.");
        
        var user = _userGenerator.GeneratedEntities[index];

        var githubName = _faker.Person.UserName;
        var association = new GithubUserAssociation(user, githubName);

        user.AddAssociation(association);

        return association;
    }
}
