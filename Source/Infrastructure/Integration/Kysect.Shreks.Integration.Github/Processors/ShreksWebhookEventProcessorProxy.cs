using Kysect.Shreks.Application.Commands.Parsers;
using Kysect.Shreks.Application.Dto.Github;
using Kysect.Shreks.Integration.Github.Applicaiton;
using Kysect.Shreks.Integration.Github.Client;
using Kysect.Shreks.Integration.Github.Helpers;
using Kysect.Shreks.Integration.Github.Notifiers;
using MediatR;
using Microsoft.Extensions.Logging;
using Octokit;
using Octokit.Webhooks;
using Octokit.Webhooks.Events;
using Octokit.Webhooks.Events.IssueComment;
using Octokit.Webhooks.Events.PullRequest;
using Octokit.Webhooks.Events.PullRequestReview;
using Octokit.Webhooks.Models;
using PullRequestReviewEvent = Octokit.Webhooks.Events.PullRequestReviewEvent;

namespace Kysect.Shreks.Integration.Github.Processors;

public sealed class ShreksWebhookEventProcessorProxy : WebhookEventProcessor
{
    private readonly IActionNotifier _actionNotifier;
    private readonly ILogger<ShreksWebhookEventProcessorProxy> _logger;
    private readonly IOrganizationGithubClientProvider _clientProvider;
    private readonly ShreksWebhookEventProcessor _processor;

    public ShreksWebhookEventProcessorProxy(
        IActionNotifier actionNotifier,
        ILogger<ShreksWebhookEventProcessorProxy> logger,
        IShreksCommandParser commandParser,
        IMediator mediator,
        IOrganizationGithubClientProvider clientProvider)
    {
        _actionNotifier = actionNotifier;
        _logger = logger;
        _clientProvider = clientProvider;
        _processor = new ShreksWebhookEventProcessor(commandParser, mediator);
    }

    protected override async Task ProcessPullRequestWebhookAsync(
        WebhookHeaders headers,
        PullRequestEvent pullRequestEvent,
        PullRequestAction action)
    {
        GithubPullRequestDescriptor githubPullRequestDescriptor = CreateDescriptor(pullRequestEvent);
        ILogger repositoryLogger = _logger.ToRepositoryLogger(githubPullRequestDescriptor);

        if (IsSenderBotOrNull(pullRequestEvent))
        {
            repositoryLogger.LogTrace($"{nameof(ProcessPullRequestWebhookAsync)} was skipped because sender is bot or null");
            return;
        }

        string pullRequestAction = action;
        repositoryLogger.LogInformation($"{nameof(ProcessPullRequestWebhookAsync)}: {pullRequestEvent.GetType().Name} with type {pullRequestAction}");
        var pullRequestCommitEventNotifier = new PullRequestCommitEventNotifier(_actionNotifier, pullRequestEvent, pullRequestEvent.PullRequest.Head.Sha, pullRequestEvent.PullRequest.Number, repositoryLogger);

        try
        {
            await _processor.ProcessPullRequestWebhookAsync(pullRequestEvent, action, githubPullRequestDescriptor, repositoryLogger, pullRequestCommitEventNotifier);
        }
        catch (Exception e)
        {

            string message = $"Failed to handle {pullRequestAction}";
            repositoryLogger.LogError(e, $"{nameof(ProcessPullRequestWebhookAsync)}: {message}");
            await pullRequestCommitEventNotifier.SendExceptionMessageSafe(e, repositoryLogger);
        }
    }

    protected override async Task ProcessPullRequestReviewWebhookAsync(
        WebhookHeaders headers,
        PullRequestReviewEvent pullRequestReviewEvent,
        PullRequestReviewAction action)
    {
        GithubPullRequestDescriptor githubPullRequestDescriptor = CreateDescriptor(pullRequestReviewEvent);
        ILogger repositoryLogger = _logger.ToRepositoryLogger(githubPullRequestDescriptor);

        if (IsSenderBotOrNull(pullRequestReviewEvent))
        {
            repositoryLogger.LogTrace($"{nameof(ProcessPullRequestReviewWebhookAsync)} was skipped because sender is bot or null");
            return;
        }

        string pullRequestReviewAction = action;
        repositoryLogger.LogInformation($"{nameof(ProcessPullRequestReviewWebhookAsync)}: {pullRequestReviewEvent.GetType().Name} with type {pullRequestReviewAction}");
        var pullRequestEventNotifier = new PullRequestEventNotifier(_actionNotifier, pullRequestReviewEvent, pullRequestReviewEvent.PullRequest.Number, repositoryLogger);

        try
        {
            await _processor.ProcessPullRequestReviewWebhookAsync(pullRequestReviewEvent, action, githubPullRequestDescriptor, repositoryLogger, pullRequestEventNotifier);
        }
        catch (Exception e)
        {
            string message = $"Failed to handle {pullRequestReviewAction}";
            repositoryLogger.LogError(e, $"{nameof(ProcessPullRequestReviewWebhookAsync)}:{message}");
            int issueNumber = (int)pullRequestReviewEvent.PullRequest.Number;
            await pullRequestEventNotifier.SendExceptionMessageSafe(e, repositoryLogger);
        }
    }

    protected override async Task ProcessIssueCommentWebhookAsync(
        WebhookHeaders headers,
        IssueCommentEvent issueCommentEvent,
        IssueCommentAction action)
    {
        GithubPullRequestDescriptor githubPullRequestDescriptor = await GetPullRequestDescriptor(issueCommentEvent);
        ILogger repositoryLogger = _logger.ToRepositoryLogger(githubPullRequestDescriptor);

        if (IsSenderBotOrNull(issueCommentEvent))
        {
            repositoryLogger.LogTrace($"{nameof(ProcessIssueCommentWebhookAsync)} was skipped because sender is bot or null");
            return;
        }

        if (!IsPullRequestCommand(issueCommentEvent))
        {
            repositoryLogger.LogTrace($"Skipping commit in {issueCommentEvent.Issue.Id}. Issue comments is not supported.");
            return;
        }

        string issueCommentAction = action;
        repositoryLogger.LogInformation($"{nameof(ProcessIssueCommentWebhookAsync)}: {issueCommentEvent.GetType().Name} with type {issueCommentAction}");
        var pullRequestCommentEventNotifier = new PullRequestCommentEventNotifier(_actionNotifier, issueCommentEvent, issueCommentEvent.Comment.Id, issueCommentEvent.Issue.Number, repositoryLogger);

        try
        {
            await _processor.ProcessIssueCommentWebhookAsync(issueCommentEvent, action, githubPullRequestDescriptor, repositoryLogger, pullRequestCommentEventNotifier);
        }
        catch (Exception e)
        {
            string message = $"Failed to handle {issueCommentAction}";
            repositoryLogger.LogError(e, $"{nameof(ProcessIssueCommentWebhookAsync)}: {message}");
            await pullRequestCommentEventNotifier.SendExceptionMessageSafe(e, repositoryLogger);
        }
    }

    private bool IsPullRequestCommand(IssueCommentEvent issueCommentEvent)
    {
        return issueCommentEvent.Issue.PullRequest.Url is not null;
    }

    private bool IsSenderBotOrNull(WebhookEvent webhookEvent)
    {
        return webhookEvent.Sender is null || webhookEvent.Sender.Type == UserType.Bot;
    }

    public GithubPullRequestDescriptor CreateDescriptor(PullRequestEvent @event)
    {
        string login = @event.Sender!.Login;
        string payload = @event.PullRequest.HtmlUrl;
        string organization = @event.Organization!.Login;
        string repository = @event.Repository!.Name;
        string branch = @event.PullRequest.Head.Ref;
        long prNum = @event.PullRequest.Number;

        var pullRequestDescriptor = new GithubPullRequestDescriptor(
            login,
            payload,
            organization,
            repository,
            branch,
            prNum);

        return pullRequestDescriptor;
    }

    public GithubPullRequestDescriptor CreateDescriptor(PullRequestReviewEvent pullRequestReviewEvent)
    {
        return new GithubPullRequestDescriptor(
            pullRequestReviewEvent.Sender!.Login,
            Payload: pullRequestReviewEvent.Review.HtmlUrl,
            pullRequestReviewEvent.Organization!.Login,
            pullRequestReviewEvent.Repository!.Name,
            BranchName: pullRequestReviewEvent.PullRequest.Head.Ref,
            pullRequestReviewEvent.PullRequest.Number);
    }

    private async Task<GithubPullRequestDescriptor> GetPullRequestDescriptor(IssueCommentEvent issueCommentEvent)
    {
        ArgumentNullException.ThrowIfNull(issueCommentEvent.Sender);
        ArgumentNullException.ThrowIfNull(issueCommentEvent.Organization);
        ArgumentNullException.ThrowIfNull(issueCommentEvent.Repository);

        GitHubClient gitHubClient = await _clientProvider.GetClient(issueCommentEvent.Organization.Login);
        PullRequest pullRequest = await gitHubClient.PullRequest
            .Get(issueCommentEvent.Repository.Id, (int)issueCommentEvent.Issue.Number);

        return new GithubPullRequestDescriptor(
            issueCommentEvent.Sender.Login,
            Payload: pullRequest.HtmlUrl,
            issueCommentEvent.Organization.Login,
            issueCommentEvent.Repository.Name,
            BranchName: pullRequest.Head.Ref,
            pullRequest.Number);
    }
}