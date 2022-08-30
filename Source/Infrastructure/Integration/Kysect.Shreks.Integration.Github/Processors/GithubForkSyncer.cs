using Kysect.Shreks.Application.Abstractions.Github.Queries;
using Kysect.Shreks.Application.Dto.Github;
using Kysect.Shreks.Integration.Github.Client;
using MediatR;
using Octokit;

namespace Kysect.Shreks.Integration.Github.Processors;

public class GithubForkSyncer
{
    private readonly IMediator _mediator;
    private readonly IOrganizationGithubClientProvider _clientProvider;

    public GithubForkSyncer(IMediator mediator, IOrganizationGithubClientProvider clientProvider)
    {
        _mediator = mediator;
        _clientProvider = clientProvider;
    }

    public async Task<Boolean> IsTemplateRepo(GithubPullRequestDescriptor pullRequestDescriptor)
    {
        var request = new FindSubjectCourseTemplateRepositoryName.Query(pullRequestDescriptor.Organization);
        FindSubjectCourseTemplateRepositoryName.Response response = await _mediator.Send(request);

        if (response.TemplateRepositoryName is null)
            return false;

        return string.Equals(response.TemplateRepositoryName, pullRequestDescriptor.Repository);
    }

    public async Task SyncRepos(GithubPullRequestDescriptor pullRequestDescriptor)
    {
        GitHubClient gitHubClient = await _clientProvider.GetClient(pullRequestDescriptor.Organization);

        IReadOnlyList<Repository> repositories = await gitHubClient.Repository.GetAllForOrg(pullRequestDescriptor.Organization);

        foreach (Repository repository in repositories)
        {
            // /repos/{owner}/{repo}/merge-upstream
        }
    }
}