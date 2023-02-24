using ITMO.Dev.ASAP.Application.GithubWorkflow.Abstractions.Client;
using ITMO.Dev.ASAP.Application.GithubWorkflow.Abstractions.Providers;
using Octokit;

namespace ITMO.Dev.ASAP.Integration.Github.Providers;

public class GithubUserProvider : IGithubUserProvider
{
    private readonly IServiceOrganizationGithubClientProvider _clientProvider;

    public GithubUserProvider(IServiceOrganizationGithubClientProvider clientProvider)
    {
        _clientProvider = clientProvider;
    }

    public async Task<bool> IsGithubUserExists(string username)
    {
        ArgumentNullException.ThrowIfNull(username);

        GitHubClient client = await _clientProvider.GetClient();

        try
        {
            User user = await client.User.Get(username);
            return user.Login.Equals(username, StringComparison.OrdinalIgnoreCase);
        }
        catch (NotFoundException)
        {
            return false;
        }
    }
}