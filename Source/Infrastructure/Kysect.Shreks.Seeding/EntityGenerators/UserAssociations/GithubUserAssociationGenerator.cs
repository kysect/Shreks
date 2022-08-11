using Bogus;
using Kysect.Shreks.Core.UserAssociations;
using Kysect.Shreks.Core.Users;
using Kysect.Shreks.Seeding.Options;

namespace Kysect.Shreks.Seeding.EntityGenerators;

public class GithubUserAssociationGenerator : EntityGeneratorBase<GithubUserAssociation>
{
    private readonly IEntityGenerator<Mentor> _mentorGenerator;
    private readonly IEntityGenerator<Student> _studentGenerator;
    
    private readonly Faker _faker;

    public GithubUserAssociationGenerator(
        EntityGeneratorOptions<GithubUserAssociation> options,
        IEntityGenerator<Mentor> mentorGenerator,
        IEntityGenerator<Student> studentGenerator,
        Faker faker) : base(options)
    {
        _mentorGenerator = mentorGenerator;
        _studentGenerator = studentGenerator;
        _faker = faker;
    }

    protected override GithubUserAssociation Generate(int index)
    {
        var generatedUsers = _mentorGenerator.GeneratedEntities
            .Concat<User>(_studentGenerator.GeneratedEntities)
            .ToList();

        if (index >= generatedUsers.Count)
            throw new IndexOutOfRangeException("User index more than count of users.");
        
        var user = generatedUsers[index];

        var githubName = _faker.Internet.UserName(user.FirstName, user.LastName);
        var association = new GithubUserAssociation(user, githubName);

        user.AddAssociation(association);

        return association;
    }
}
