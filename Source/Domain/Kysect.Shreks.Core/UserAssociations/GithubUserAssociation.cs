using Kysect.Shreks.Core.Users;

namespace Kysect.Shreks.Core.UserAssociations;

public partial class GithubUserAssociation : UserAssociation
{
    public GithubUserAssociation(Guid id, User user, string githubUsername) : base(id, user)
    {
        GithubUsername = githubUsername;
    }

    public string GithubUsername { get; set; }
}