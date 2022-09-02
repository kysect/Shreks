using Kysect.Shreks.Application.Abstractions.Github.Queries;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Kysect.Shreks.Integration.Github.Invites;

public class GithubInvitingWorker : BackgroundService
{
    /// <summary>
    /// This worker is our restriction bypass, github allow to invite only 50 users per 24 hours.
    /// So we need to send invites every 24 hours + 1 minutes shift for preventing race conditions.
    /// </summary>
    private readonly TimeSpan _delayBetweenInviteIteration = TimeSpan.FromHours(24).Add(TimeSpan.FromMinutes(1));

    private readonly ILogger<GithubInvitingWorker> _logger;
    private readonly IServiceScopeFactory _serviceProvider;

    public GithubInvitingWorker(IServiceScopeFactory serviceProvider,
        ILogger<GithubInvitingWorker> logger)
    {
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
                    var inviteSender = scope.ServiceProvider.GetRequiredService<SubjectCourseGithubOrganizationInviteSender>();
                    var repositoryManager = scope.ServiceProvider.GetRequiredService<ISubjectCourseGithubOrganizationRepositoryManager>();

                    var response = await mediator.Send(query, stoppingToken);

                    foreach (var association in response.Associations)
                    {
                        var (subjectCourseId, courseName, organizationName, templateRepositoryName) = association;

                        var usernamesQuery = new GetGithubUsernamesForSubjectCourse.Query(subjectCourseId);
                        var usernames = await mediator.Send(usernamesQuery, stoppingToken);

                        await inviteSender.Invite(organizationName, usernames.StudentGithubUsernames);
                        await GenerateRepositories(repositoryManager, usernames.StudentGithubUsernames, organizationName, templateRepositoryName);
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

    private async Task GenerateRepositories(
        ISubjectCourseGithubOrganizationRepositoryManager repositoryManager,
        IReadOnlyCollection<string> usernames,
        string organizationName,
        string templateName)
    {
        IReadOnlyCollection<string> repositories = await repositoryManager.GetRepositories(organizationName);

        if (!repositories.Contains(templateName))
        {
            string message = $"No template repository found for organization {organizationName}";
            _logger.LogWarning(message);
            return;
        }

        foreach (string username in usernames)
        {
            string newRepositoryName = username;

            if (repositories.Any(r => r.Equals(newRepositoryName, StringComparison.OrdinalIgnoreCase)))
                continue;
            
            await repositoryManager.CreateRepositoryFromTemplate(organizationName, newRepositoryName, templateName);
            await repositoryManager.AddAdminPermission(organizationName, newRepositoryName, username);

            _logger.LogInformation("Successfully created repository for user {User}", username);
        }
    }
}