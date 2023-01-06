using Kysect.CommonLib;
using Kysect.GithubUtils;
using Kysect.Shreks.Application.GithubWorkflow.Abstractions;
using Kysect.Shreks.Application.GithubWorkflow.Abstractions.Client;
using Microsoft.Extensions.Logging;
using Octokit;

namespace Kysect.Shreks.Integration.Github.Invites;

public class SubjectCourseGithubOrganizationInviteSender : ISubjectCourseGithubOrganizationInviteSender
{
    private readonly IOrganizationGithubClientProvider _clientProvider;
    private readonly ILogger<SubjectCourseGithubOrganizationInviteSender> _logger;

    public SubjectCourseGithubOrganizationInviteSender(
        ILogger<SubjectCourseGithubOrganizationInviteSender> logger,
        IOrganizationGithubClientProvider clientProvider)
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
            IReadOnlyCollection<string> invites = inviteResult
                .Where(result => result.Result is UserInviteResultType.Success)
                .Select(invite => invite.Username)
                .ToList();

            int count = invites.Count;
            string users = invites.ToSingleString();
            _logger.LogInformation("Success invites: {InviteCount}. Users: {Users}", count, users);
        }

        if (inviteResult.Any(result => result.Result is UserInviteResultType.AlreadyAdded))
        {
            IReadOnlyCollection<string> invites = inviteResult
                .Where(result => result.Result is UserInviteResultType.AlreadyAdded)
                .Select(invite => invite.Username)
                .ToList();

            int count = invites.Count;
            string users = invites.ToSingleString();
            _logger.LogInformation("AlreadyAdded invites: {AlreadyAddedCount}. Users: {Users}", count, users);
        }

        if (inviteResult.Any(result => result.Result is UserInviteResultType.AlreadyInvited))
        {
            IReadOnlyCollection<string> invites = inviteResult
                .Where(result => result.Result is UserInviteResultType.AlreadyInvited)
                .Select(invite => invite.Username)
                .ToList();

            int count = invites.Count;
            string users = invites.ToSingleString();
            _logger.LogInformation("AlreadyInvitedCount invites: {AlreadyInvitedCount}. Users: {Users}", count, users);
        }

        if (inviteResult.Any(result => result.Result is UserInviteResultType.InvitationExpired))
        {
            IReadOnlyCollection<string> invites = inviteResult
                .Where(result => result.Result is UserInviteResultType.InvitationExpired)
                .Select(invite => invite.Username)
                .ToList();

            int count = invites.Count;
            string users = invites.ToSingleString();
            _logger.LogInformation("InvitationExpired invites: {Count}. Users: {Users}", count, users);
        }

        if (inviteResult.Any(result => result.Result is UserInviteResultType.Failed))
        {
            IReadOnlyCollection<UserInviteResult> invites = inviteResult
                .Where(result => result.Result is UserInviteResultType.Failed)
                .ToList();

            int count = invites.Count;
            string? error = invites.First(invite => invite.Reason is not null).Reason;
            _logger.LogInformation("Failed invites: {FailedCount}. Error: {Error}", count, error);
        }
    }

    private async Task<OrganizationInviteSender> CreateOrganizationInviteSender(string githubOrganization)
    {
        GitHubClient client = await _clientProvider.GetClient(githubOrganization);

        return new OrganizationInviteSender(client);
    }
}