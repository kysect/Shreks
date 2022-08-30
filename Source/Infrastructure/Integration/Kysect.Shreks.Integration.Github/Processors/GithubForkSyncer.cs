using System.Net;
using Kysect.Shreks.Application.Abstractions.Github.Queries;
using Kysect.Shreks.Application.Dto.Github;
using Kysect.Shreks.Integration.Github.Client;
using MediatR;
using Microsoft.Extensions.Logging;
using Octokit;

namespace Kysect.Shreks.Integration.Github.Processors;

public class GithubForkSyncer
{
    private readonly IMediator _mediator;
    private readonly IOrganizationGithubClientProvider _clientProvider;
    private readonly ILogger _logger;

    public GithubForkSyncer(IMediator mediator, IOrganizationGithubClientProvider clientProvider, ILogger logger)
    {
        _mediator = mediator;
        _clientProvider = clientProvider;
        _logger = logger;
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
        _logger.LogInformation($"Sync repos in org {pullRequestDescriptor.Organization} with changes in template repo.");

        GitHubClient gitHubClient = await _clientProvider.GetClient(pullRequestDescriptor.Organization);

        IReadOnlyList<Repository> repositories = await gitHubClient.Repository.GetAllForOrg(pullRequestDescriptor.Organization);
        _logger.LogDebug($"Org {pullRequestDescriptor.Organization} has {repositories.Count} repositories for syncing.");

        foreach (Repository repository in repositories)
        {
            HttpStatusCode httpStatusCode = await gitHubClient.Connection.Post(new Uri($"/repos/{repository.Owner.Login}/{repository.Name}/merge-upstream"));
            if (httpStatusCode == HttpStatusCode.OK)
            {
                _logger.LogInformation($"Repository {repository.FullName} was updated with template repo.");
                continue;
            }

            if (httpStatusCode == HttpStatusCode.Conflict)
            {
                _logger.LogWarning($"Repository {repository.FullName} cannot be updated with template. The branch could not be synced because of a merge conflict.");
                continue;
            }

            _logger.LogWarning($"Repository {repository.FullName} cannot be updated with template. The branch could not be synced for some other reason.");
        }
    }
}