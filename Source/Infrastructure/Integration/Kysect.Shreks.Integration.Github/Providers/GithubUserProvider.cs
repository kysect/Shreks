using Kysect.Shreks.Application.GithubWorkflow.Abstractions;
using Octokit;

namespace Kysect.Shreks.Integration.Github.Providers;

public class GithubUserProvider : IGithubUserProvider
{
    private readonly IGitHubClient _appClient;

    public GithubUserProvider(IGitHubClient appClient)
    {
        _appClient = appClient;
    }

    public async Task<bool> IsGithubUserExists(string username)
    {
        ArgumentNullException.ThrowIfNull(username);

        try
        {
            User user = await _appClient.User.Get(username);

            return user.Login.Equals(username, StringComparison.Ordinal);
        }
        catch (NotFoundException)
        {
            return false;
        }
    }
}