namespace Kysect.Shreks.Application.GithubWorkflow.Abstractions;

public interface IGithubUserProvider
{
    Task<bool> IsGithubUserExists(string username);
}