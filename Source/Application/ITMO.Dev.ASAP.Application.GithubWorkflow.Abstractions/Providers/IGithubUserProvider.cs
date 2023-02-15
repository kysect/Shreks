namespace ITMO.Dev.ASAP.Application.GithubWorkflow.Abstractions.Providers;

public interface IGithubUserProvider
{
    Task<bool> IsGithubUserExists(string username);
}