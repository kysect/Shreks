using Kysect.Shreks.Core.UserAssociations;
using Kysect.Shreks.Core.Users;

namespace Kysect.Shreks.Core.Specifications.Github;

public class FindUserByGithubUsername : ISpecification<UserAssociation, User>
{
    private readonly string _githubUsername;

    public FindUserByGithubUsername(string githubUsername)
    {
        _githubUsername = githubUsername;
    }

    public IQueryable<User> Apply(IQueryable<UserAssociation> query)
    {
        return query
            .OfType<GithubUserAssociation>()
            .Where(u => u.GithubUsername == _githubUsername)
            .Select(a => a.User);
    }
}