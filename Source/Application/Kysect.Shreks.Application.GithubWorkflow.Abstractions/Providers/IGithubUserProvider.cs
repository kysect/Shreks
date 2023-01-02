namespace Kysect.Shreks.Application.GithubWorkflow.Abstractions.Providers;

public interface IGithubUserProvider
{
    Task<bool> IsGithubUserExists(string username);
}