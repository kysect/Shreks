using Kysect.Shreks.Application.GithubWorkflow.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Kysect.Shreks.Application.GithubWorkflow.BackgroundServices;

public class GithubInvitingWorker : BackgroundService
{
    /// <summary>
    ///     This worker is our restriction bypass, github allow to invite only 50 users per 24 hours.
    ///     So we need to send invites every 24 hours + 1 minutes shift for preventing race conditions.
    ///     But in case of restart it is better to try many time. Anyways we have logic for stop inviting after first fail.
    /// </summary>
    private readonly TimeSpan _delayBetweenInviteIteration = TimeSpan.FromHours(6);

    private readonly ILogger<GithubInvitingWorker> _logger;
    private readonly IServiceScopeFactory _serviceProvider;

    public GithubInvitingWorker(
        IServiceScopeFactory serviceProvider,
        ILogger<GithubInvitingWorker> logger)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(_delayBetweenInviteIteration);

        while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(stoppingToken))
        {
            try
            {
                using IServiceScope scope = _serviceProvider.CreateScope();

                ISubjectCourseGithubOrganizationManager subjectCourseGithubOrganizationManager =
                    scope.ServiceProvider.GetRequiredService<ISubjectCourseGithubOrganizationManager>();
                await subjectCourseGithubOrganizationManager.UpdateOrganizations(stoppingToken);
            }
            catch (Exception ex)
            {
                const string template = "Failed to execute GithubInvitingWorker with exception message {Message}.";
                _logger.LogError(ex, template, ex.Message);
            }
        }
    }
}