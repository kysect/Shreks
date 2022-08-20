using Kysect.Shreks.Application.Abstractions.Exceptions;
using Kysect.Shreks.Application.Abstractions.Github.Commands;
using Kysect.Shreks.Core.SubjectCourseAssociations;
using Kysect.Shreks.Core.UserAssociations;
using Kysect.Shreks.DataAccess.Abstractions;
using Kysect.Shreks.Integration.Github.Client;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Octokit;
using Kysect.Shreks.Core.Study;
using Microsoft.Extensions.Logging;

namespace Kysect.Shreks.Application.Handlers.Github;

public class SyncGithubAdminWithMentorsHandler : IRequestHandler<SyncGithubAdminWithMentors.Command>
{
    private readonly IShreksDatabaseContext _shreksDatabaseContext;
    private readonly IInstallationClientFactory _clientFactory;
    private readonly IGitHubClient _appClient;
    private readonly ILogger<SyncGithubAdminWithMentorsHandler> _logger;

    public SyncGithubAdminWithMentorsHandler(
        IShreksDatabaseContext shreksDatabaseContext,
        IInstallationClientFactory clientFactory,
        IGitHubClient appClient,
        ILogger<SyncGithubAdminWithMentorsHandler> logger)
    {
        _shreksDatabaseContext = shreksDatabaseContext;
        _clientFactory = clientFactory;
        _appClient = appClient;
        _logger = logger;
    }

    public async Task<Unit> Handle(SyncGithubAdminWithMentors.Command request, CancellationToken cancellationToken)
    {
        GithubSubjectCourseAssociation? courseGithub = await _shreksDatabaseContext
            .SubjectCourseAssociations
            .OfType<GithubSubjectCourseAssociation>()
            .FirstOrDefaultAsync(a => a.GithubOrganizationName == request.OrganizationName, cancellationToken);

        if (courseGithub is null)
            throw new EntityNotFoundException($"Cannot found organization {request.OrganizationName} in database");

        IReadOnlyList<User> organizationAdmins = await GetOrganizationAdmins(courseGithub.GithubOrganizationName);
        HashSet<string> mentorUsernames = await GetMentorUsernames(courseGithub.SubjectCourse.Id, cancellationToken);

        List<User> notMentorAdmins = organizationAdmins.Where(a => !mentorUsernames.Contains(a.Login)).ToList();
        if (!notMentorAdmins.Any())
        {
            _logger.LogWarning($"All github admins ({organizationAdmins.Count}) already added as mentors.");
            return Unit.Value;
        }

        foreach (User adminUser in notMentorAdmins)
        {
            _logger.LogInformation($"Github user {adminUser.Login} is admin and will be added as mentor to {courseGithub.SubjectCourse.Name}");
            await CreateMentorFromAdmin(courseGithub.SubjectCourse, adminUser.Login, cancellationToken);
        }

        await _shreksDatabaseContext.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }

    private async Task<IReadOnlyList<User>> GetOrganizationAdmins(string githubOrganization)
    {
        Installation installation = await _appClient.GitHubApps.GetOrganizationInstallationForCurrent(githubOrganization);
        GitHubClient client = _clientFactory.GetClient(installation.Id);
        IReadOnlyList<User> adminUsers = await client.Organization.Member.GetAll(githubOrganization, OrganizationMembersRole.Admin);
        return adminUsers;
    }

    private async Task<HashSet<string>> GetMentorUsernames(Guid subjectCourseId, CancellationToken cancellationToken)
    {
        List<string> currentMentors = await _shreksDatabaseContext
            .SubjectCourses
            .Where(s => s.Id == subjectCourseId)
            .SelectMany(a => a.Mentors)
            .Select(m => m.User.Associations)
            .OfType<GithubUserAssociation>()
            .Select(a => a.GithubUsername)
            .ToListAsync(cancellationToken);

        return currentMentors.ToHashSet();
    }

    private async Task CreateMentorFromAdmin(SubjectCourse subjectCourse, string adminUsername, CancellationToken cancellationToken)
    {
        GithubUserAssociation? userAssociation = await _shreksDatabaseContext
            .Users
            .SelectMany(u => u.Associations)
            .OfType<GithubUserAssociation>()
            .FirstOrDefaultAsync(cancellationToken);

        if (userAssociation is not null)
        {
            subjectCourse.AddMentor(userAssociation.User);
        }
        else
        {
            var adminUser = new Core.Users.User(adminUsername, adminUsername, adminUsername);
            var githubUserAssociation = new GithubUserAssociation(adminUser, adminUsername);

            _shreksDatabaseContext.Users.Add(adminUser);
            _shreksDatabaseContext.UserAssociations.Add(githubUserAssociation);
            subjectCourse.AddMentor(adminUser);
        }
    }
}