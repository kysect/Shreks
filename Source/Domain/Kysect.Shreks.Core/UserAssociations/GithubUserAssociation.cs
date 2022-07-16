using Kysect.Shreks.Core.Users;

namespace Kysect.Shreks.Core.UserAssociations;

public partial class GithubUserAssociation : UserAssociation
{
    public GithubUserAssociation(User user, string githubUsername) : base(user)
    {
        GithubUsername = githubUsername;
    }

    public string GithubUsername { get; set; }
}