using Kysect.CommonLib;
using Kysect.GithubUtils;
using Kysect.Shreks.Application.Abstractions.DataAccess;
using Kysect.Shreks.Application.Abstractions.Students;
using Kysect.Shreks.Core.Study;
using Kysect.Shreks.Core.SubjectCourseAssociations;
using Kysect.Shreks.Integration.Github.Client;
using MediatR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Octokit;

namespace Kysect.Shreks.Application.GithubWorker
{
    public class GithubInvitingWorker : BackgroundService
    {
        /// <summary>
        /// This worker is our restriction bypass, github allow to invite only 50 users per 24 hours.
        /// So we need to send invites every 24 hours + 1 minutes shift for preventing race conditions.
        /// </summary>
        private readonly TimeSpan _delayBetweenInviteIteration = TimeSpan.FromHours(24).Add(TimeSpan.FromMinutes(1));

        private readonly ILogger<GithubInvitingWorker> _logger;
        private readonly IInstallationClientFactory _clientFactory;
        private readonly IShreksDatabaseContext _context;
        private readonly IGitHubClient _appClient;
        private readonly IMediator _mediator;

        public GithubInvitingWorker(ILogger<GithubInvitingWorker> logger, IInstallationClientFactory clientFactory, IShreksDatabaseContext context, IGitHubClient appClient, IMediator mediator)
        {
            _logger = logger;
            _clientFactory = clientFactory;
            _context = context;
            _appClient = appClient;
            _mediator = mediator;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var timer = new PeriodicTimer(_delayBetweenInviteIteration);
            while (!stoppingToken.IsCancellationRequested &&
                   await timer.WaitForNextTickAsync(stoppingToken))
            {
                try
                {
                    await foreach (SubjectCourseAssociation subjectCourseAssociation in _context.SubjectCourseAssociations)
                    {
                        if (subjectCourseAssociation is not GithubSubjectCourseAssociation githubSubjectCourseAssociation)
                            continue;

                        await ProcessSubject(githubSubjectCourseAssociation, stoppingToken);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Failed to execute GithubInvitingWorker with exception message {ex.Message}.");
                }
            }
        }

        private async Task ProcessSubject(GithubSubjectCourseAssociation githubSubjectCourseAssociation, CancellationToken stoppingToken)
        {
            SubjectCourse subjectCourse = githubSubjectCourseAssociation.SubjectCourse;
            var message = $"Start inviting to organization {githubSubjectCourseAssociation.GithubOrganizationName} for course {subjectCourse.Subject.Title}";
            _logger.LogInformation(message);

            GetGithubUsernamesForSubjectCourse.Response response = await _mediator.Send(new GetGithubUsernamesForSubjectCourse.Query(subjectCourse.Id), stoppingToken);
            IReadOnlyCollection<string> organizationUsers = response.StudentGithubUsernames;
            OrganizationInviteSender organizationInviteSender = await CreateOrganizationInviteSender(githubSubjectCourseAssociation.GithubOrganizationName);

            InviteResult inviteResult = await organizationInviteSender.Invite(githubSubjectCourseAssociation.GithubOrganizationName, organizationUsers);
            
            _logger.LogInformation($"Finish invite cycle for organization {githubSubjectCourseAssociation.GithubOrganizationName}");
            if (inviteResult.Success.Any())
                _logger.LogInformation($"Success invites: {inviteResult.Success.Count}. Users: {inviteResult.Success.ToSingleString()}");
            if (inviteResult.AlreadyAdded.Any())
                _logger.LogInformation($"Success invites: {inviteResult.AlreadyAdded.Count}. Users: {inviteResult.AlreadyAdded.ToSingleString()}");
            if (inviteResult.AlreadyInvited.Any())
                _logger.LogInformation($"Success invites: {inviteResult.AlreadyInvited.Count}. Users: {inviteResult.AlreadyInvited.ToSingleString()}");
            if (inviteResult.WithExpiredInvites.Any())
                _logger.LogInformation($"Success invites: {inviteResult.WithExpiredInvites.Count}. Users: {inviteResult.WithExpiredInvites.ToSingleString()}");
            if (inviteResult.Failed.Any())
                _logger.LogInformation($"Success invites: {inviteResult.Failed.Count}. Error: {inviteResult.Exception?.ToString()}");
        }

        private async Task<OrganizationInviteSender> CreateOrganizationInviteSender(string githubOrganization)
        {
            Installation installation = await _appClient.GitHubApps.GetOrganizationInstallationForCurrent(githubOrganization);
            GitHubClient client = await _clientFactory.GetClient(installation.Id);
            return new OrganizationInviteSender(client);
        }
    }
}