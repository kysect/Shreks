using ITMO.Dev.ASAP.Core.Users;

namespace ITMO.Dev.ASAP.Core.UserAssociations;

public partial class GithubUserAssociation : UserAssociation
{
    public GithubUserAssociation(Guid id, User user, string githubUsername) : base(id, user)
    {
        GithubUsername = githubUsername;
    }

    public string GithubUsername { get; set; }
}