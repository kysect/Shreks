using Kysect.CommonLib;
using Kysect.GithubUtils;
using Kysect.Shreks.Application.Abstractions.Github.Queries;
using Kysect.Shreks.Application.Abstractions.Students;
using Kysect.Shreks.Application.Dto.SubjectCourseAssociations;
using Kysect.Shreks.Integration.Github.Client;
using MediatR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Octokit;

namespace Kysect.Shreks.Integration.Github.Invites;

public class GithubInvitingWorker : BackgroundService
{
    /// <summary>
    /// This worker is our restriction bypass, github allow to invite only 50 users per 24 hours.
    /// So we need to send invites every 24 hours + 1 minutes shift for preventing race conditions.
    /// </summary>
    private readonly TimeSpan _delayBetweenInviteIteration = TimeSpan.FromHours(24).Add(TimeSpan.FromMinutes(1));

    private readonly ILogger<GithubInvitingWorker> _logger;
    private readonly IOrganizationGithubClientProvider _clientProvider;
    private readonly IMediator _mediator;

    public GithubInvitingWorker(
        IOrganizationGithubClientProvider clientProvider,
        IMediator mediator,
        ILogger<GithubInvitingWorker> logger)
    {
        _clientProvider = clientProvider;
        _logger = logger;
        _mediator = mediator;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(_delayBetweenInviteIteration);
        var query = new GetSubjectCourseGithubAssociations.Query();

        while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(stoppingToken))
        {
            try
            {
                var response = await _mediator.Send(query, stoppingToken);

                foreach (var association in response.Associations)
                {
                    await ProcessSubject(association, stoppingToken);
                }
            }
            catch (Exception ex)
            {
                const string template = "Failed to execute GithubInvitingWorker with exception message {Message}.";
                _logger.LogError(ex, template, ex.Message);
            }
        }
    }

    private async Task ProcessSubject(GithubSubjectCourseAssociationDto association, CancellationToken stoppingToken)
    {
        const string template = "Start inviting to organization {OrganizationName} for course {CourseName}";

        var (subjectCourseId, courseName, organizationName) = association;

        _logger.LogInformation(template, organizationName, courseName);

        var usernamesQuery = new GetGithubUsernamesForSubjectCourse.Query(subjectCourseId);
        var response = await _mediator.Send(usernamesQuery, stoppingToken);

        IReadOnlyCollection<string> organizationUsers = response.StudentGithubUsernames;
        var inviteSender = await CreateOrganizationInviteSender(organizationName);
        var inviteResult = await inviteSender.Invite(organizationName, organizationUsers);

        _logger.LogInformation("Finish invite cycle for organization {OrganizationName}", organizationName);

        if (inviteResult.Success.Any())
        {
            var count = inviteResult.Success.Count;
            var users = inviteResult.Success.ToSingleString();
            _logger.LogInformation("Success invites: {InviteCount}. Users: {Users}", count, users);
        }

        if (inviteResult.AlreadyAdded.Any())
        {
            var count = inviteResult.AlreadyAdded.Count;
            var users = inviteResult.AlreadyAdded.ToSingleString();
            _logger.LogInformation("Success invites: {AlreadyAddedCount}. Users: {Users}", count, users);
        }

        if (inviteResult.AlreadyInvited.Any())
        {
            var count = inviteResult.AlreadyInvited.Count;
            var users = inviteResult.AlreadyInvited.ToSingleString();
            _logger.LogInformation("Success invites: {AlreadyInvitedCount}. Users: {Users}", count, users);
        }

        if (inviteResult.WithExpiredInvites.Any())
        {
            var count = inviteResult.WithExpiredInvites.Count;
            var users = inviteResult.WithExpiredInvites.ToSingleString();
            _logger.LogInformation("Success invites: {Count}. Users: {Users}", count, users);
        }

        if (inviteResult.Failed.Any())
        {
            var count = inviteResult.Failed.Count;
            var error = inviteResult.Exception;
            _logger.LogInformation("Success invites: {FailedCount}. Error: {Error}", count, error);
        }
    }

    private async Task<OrganizationInviteSender> CreateOrganizationInviteSender(string githubOrganization)
    {
        GitHubClient client = await _clientProvider.GetClient(githubOrganization);

        return new OrganizationInviteSender(client);
    }
}