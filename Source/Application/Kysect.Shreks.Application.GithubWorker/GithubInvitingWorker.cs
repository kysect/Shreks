using Kysect.CommonLib;
using Kysect.GithubUtils;
using Kysect.Shreks.Application.Abstractions.DataAccess;
using Kysect.Shreks.Core.Study;
using Kysect.Shreks.Core.SubjectCourseAssociations;
using Kysect.Shreks.Core.UserAssociations;
using Kysect.Shreks.Integration.Github.Client;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Octokit;

namespace Kysect.Shreks.Application.GithubWorker
{
    public class GithubInvitingWorker : BackgroundService
    {
        private readonly TimeSpan _period = TimeSpan.FromHours(24).Add(TimeSpan.FromMinutes(1));

        private readonly ILogger<GithubInvitingWorker> _logger;
        private readonly IInstallationClientFactory _clientFactory;
        private readonly IShreksDatabaseContext _context;
        private readonly IGitHubClient _appClient;

        public GithubInvitingWorker(ILogger<GithubInvitingWorker> logger, IInstallationClientFactory clientFactory, IShreksDatabaseContext context, IGitHubClient appClient)
        {
            _logger = logger;
            _clientFactory = clientFactory;
            _context = context;
            _appClient = appClient;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var timer = new PeriodicTimer(_period);
            while (!stoppingToken.IsCancellationRequested &&
                   await timer.WaitForNextTickAsync(stoppingToken))
            {
                try
                {
                    await foreach (SubjectCourseAssociation subjectCourseAssociation in _context.SubjectCourseAssociations)
                    {
                        if (subjectCourseAssociation is not GithubSubjectCourseAssociation githubSubjectCourseAssociation)
                            continue;

                        await ProcessSubject(githubSubjectCourseAssociation);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Failed to execute GithubInvitingWorker with exception message {ex.Message}.");
                }
            }
        }

        private async Task ProcessSubject(GithubSubjectCourseAssociation githubSubjectCourseAssociation)
        {
            SubjectCourse subjectCourse = githubSubjectCourseAssociation.SubjectCourse;
            var message = $"Start inviting to organization {githubSubjectCourseAssociation.GithubOrganizationName} for course {subjectCourse.Subject.Title}";
            _logger.LogInformation(message);

            List<string> organizationUsers = await GetOrganizationUsers(subjectCourse);
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

        private async Task<List<string>> GetOrganizationUsers(SubjectCourse subjectCourse)
        {
            List<UserAssociation> associations = await _context
                .SubjectCourseGroups
                .Where(subjectCourseGroup => subjectCourseGroup.SubjectCourseId == subjectCourse.Id)
                .Select(group => group.StudentGroup)
                .SelectMany(group => group.Students)
                .SelectMany(student => student.Associations)
                .ToListAsync();

            return associations
                .OfType<GithubUserAssociation>()
                .Select(associations => associations.GithubUsername)
                .ToList();
        }
    }
}