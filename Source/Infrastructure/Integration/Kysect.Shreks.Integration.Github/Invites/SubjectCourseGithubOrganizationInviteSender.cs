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
        IReadOnlyCollection<UserInviteResult> inviteResult = await inviteSender.Invite(organizationName, usernames);

        _logger.LogInformation("Finish invite cycle for organization {OrganizationName}", organizationName);

        if (inviteResult.Any(result => result.Result is UserInviteResultType.Success))
        {
            IReadOnlyCollection<UserInviteResult> invites = inviteResult
                .Where(result => result.Result is UserInviteResultType.Success)
                .ToList();

            var count = invites.Count;
            var users = invites.ToSingleString();
            _logger.LogInformation("Success invites: {InviteCount}. Users: {Users}", count, users);
        }

        if (inviteResult.Any(result => result.Result is UserInviteResultType.AlreadyAdded))
        {
            IReadOnlyCollection<UserInviteResult> invites = inviteResult
                .Where(result => result.Result is UserInviteResultType.AlreadyAdded)
                .ToList();

            var count = invites.Count;
            var users = invites.ToSingleString();
            _logger.LogInformation("Success invites: {AlreadyAddedCount}. Users: {Users}", count, users);
        }

        if (inviteResult.Any(result => result.Result is UserInviteResultType.AlreadyInvited))
        {
            IReadOnlyCollection<UserInviteResult> invites = inviteResult
                .Where(result => result.Result is UserInviteResultType.AlreadyInvited)
                .ToList();

            var count = invites.Count;
            var users = invites.ToSingleString();
            _logger.LogInformation("Success invites: {AlreadyInvitedCount}. Users: {Users}", count, users);
        }

        if (inviteResult.Any(result => result.Result is UserInviteResultType.InvitationExpired))
        {
            IReadOnlyCollection<UserInviteResult> invites = inviteResult
                .Where(result => result.Result is UserInviteResultType.InvitationExpired)
                .ToList();

            var count = invites.Count;
            var users = invites.ToSingleString();
            _logger.LogInformation("Success invites: {Count}. Users: {Users}", count, users);
        }

        if (inviteResult.Any(result => result.Result is UserInviteResultType.Failed))
        {
            IReadOnlyCollection<UserInviteResult> invites = inviteResult
                .Where(result => result.Result is UserInviteResultType.Failed)
                .ToList();

            var count = invites.Count;
            var error = invites.First(invite => invite.Reason is not null).Reason;
            _logger.LogInformation("Success invites: {FailedCount}. Error: {Error}", count, error);
        }
    }

    private async Task<OrganizationInviteSender> CreateOrganizationInviteSender(string githubOrganization)
    {
        GitHubClient client = await _clientProvider.GetClient(githubOrganization);

        return new OrganizationInviteSender(client);
    }
}