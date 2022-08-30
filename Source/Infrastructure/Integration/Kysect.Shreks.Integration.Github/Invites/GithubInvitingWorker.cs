using Kysect.CommonLib;
using Kysect.GithubUtils;
using Kysect.Shreks.Application.Abstractions.Github.Queries;
using Kysect.Shreks.Application.Abstractions.Students;
using Kysect.Shreks.Application.Dto.SubjectCourseAssociations;
using Kysect.Shreks.Integration.Github.Client;
using Kysect.Shreks.Integration.Github.Exceptions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
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

    private readonly IServiceScopeFactory _serviceProvider;

    public GithubInvitingWorker(
        IOrganizationGithubClientProvider clientProvider,
        IServiceScopeFactory serviceProvider,
        ILogger<GithubInvitingWorker> logger)
    {
        _clientProvider = clientProvider;
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(_delayBetweenInviteIteration);
        var query = new GetSubjectCourseGithubAssociations.Query();

        while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(stoppingToken))
        {
            try
            {
                using (IServiceScope scope = _serviceProvider.CreateScope())
                {
                    var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

                    var response = await mediator.Send(query, stoppingToken);

                    foreach (var association in response.Associations)
                    {
                        await ProcessSubject(mediator, association, stoppingToken);
                    }
                }
            }
            catch (Exception ex)
            {
                const string template = "Failed to execute GithubInvitingWorker with exception message {Message}.";
                _logger.LogError(ex, template, ex.Message);
            }
        }
    }

    private async Task ProcessSubject(IMediator mediator, GithubSubjectCourseAssociationDto association, CancellationToken stoppingToken)
    {
        const string template = "Start inviting to organization {OrganizationName} for course {CourseName}";

        var (subjectCourseId, courseName, organizationName) = association;

        _logger.LogInformation(template, organizationName, courseName);

        var usernamesQuery = new GetGithubUsernamesForSubjectCourse.Query(subjectCourseId);
        var response = await mediator.Send(usernamesQuery, stoppingToken);

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
            await GenerateRepositoryForAdded(inviteResult, organizationName);
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

    private async Task GenerateRepositoryForAdded(InviteResult inviteResult, string organizationName)
    {
        GitHubClient client = await _clientProvider.GetClient(organizationName);
        IReadOnlyList<Repository>? repositories = await client.Repository.GetAllForOrg(organizationName);
        Repository? templateRepository = repositories.FirstOrDefault(r => r.IsTemplate);
        const string missingRepoException = "No template repository found for organization {0}";

        if (templateRepository is null)
        {
            throw new InfrastructureInvalidOperationException(string.Format(missingRepoException, organizationName));
        }

        foreach (var addedUser in inviteResult.AlreadyAdded)
        {
            var userRepository = repositories.FirstOrDefault(r => r.Name
                .Equals(addedUser, StringComparison.OrdinalIgnoreCase));

            if (userRepository is not null)
            {
                continue;
            }
            
            var userRepositoryFromTemplate = new NewRepositoryFromTemplate(addedUser)
            {
                Owner = organizationName,
                Description = null,
                Private = true,
            };
            
            await client.Repository.Generate(
                organizationName,
                templateRepository.Name,
                userRepositoryFromTemplate);

            await client.Repository.Collaborator.Add(organizationName,
                userRepositoryFromTemplate.Name,
                addedUser,
                new CollaboratorRequest(Permission.Admin));
            
            _logger.LogInformation("Successfully created repository for user {User}", addedUser);
        }
    }

    private async Task<OrganizationInviteSender> CreateOrganizationInviteSender(string githubOrganization)
    {
        GitHubClient client = await _clientProvider.GetClient(githubOrganization);

        return new OrganizationInviteSender(client);
    }
}