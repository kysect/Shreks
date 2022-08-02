using GitHubJwt;
using Octokit;

namespace Kysect.Shreks.Integration.Github.Core.CredentialStores;

public class GithubAppCredentialStore : ICredentialStore
{
    private readonly GitHubJwtFactory _gitHubJwtFactory;

    public GithubAppCredentialStore(GitHubJwtFactory gitHubJwtFactory)
    {
        _gitHubJwtFactory = gitHubJwtFactory;
    }

    public Task<Credentials> GetCredentials()
    {
        return Task.FromResult(new Credentials(_gitHubJwtFactory.CreateEncodedJwtToken(), AuthenticationType.Bearer));
    }
}