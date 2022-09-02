using Kysect.CommonLib;
using Kysect.GithubUtils;
using Kysect.Shreks.Integration.Github.Client;
using Microsoft.Extensions.Logging;
using Octokit;

namespace Kysect.Shreks.Integration.Github.Invites;

public class SubjectCourseGithubOrganizationInviteSender : ISubjectCourseGithubOrganizationInviteSender
{
    private readonly ILogger<SubjectCourseGithubOrganizationInviteSender> _logger;
    private readonly IOrganizationGithubClientProvider _clientProvider;

    public SubjectCourseGithubOrganizationInviteSender(ILogger<SubjectCourseGithubOrganizationInviteSender> logger, IOrganizationGithubClientProvider clientProvider)
    {
        _logger = logger;
        _clientProvider = clientProvider;
    }

    public async Task Invite(string organizationName, IReadOnlyCollection<string> usernames)
    {
        const string template = "Start inviting to organization {OrganizationName}.";

        _logger.LogInformation(template, organizationName);

        OrganizationInviteSender inviteSender = await CreateOrganizationInviteSender(organizationName);
        InviteResult inviteResult = await inviteSender.Invite(organizationName, usernames);

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