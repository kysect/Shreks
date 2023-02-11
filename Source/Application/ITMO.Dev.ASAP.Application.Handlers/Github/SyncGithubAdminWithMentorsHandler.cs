using ITMO.Dev.ASAP.Application.Contracts.Github.Commands;
using ITMO.Dev.ASAP.Application.GithubWorkflow.Abstractions.Providers;
using ITMO.Dev.ASAP.Common.Exceptions;
using ITMO.Dev.ASAP.Core.Study;
using ITMO.Dev.ASAP.Core.SubjectCourseAssociations;
using ITMO.Dev.ASAP.Core.UserAssociations;
using ITMO.Dev.ASAP.Core.Users;
using ITMO.Dev.ASAP.DataAccess.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ITMO.Dev.ASAP.Application.Handlers.Github;

internal class SyncGithubAdminWithMentorsHandler : IRequestHandler<SyncGithubAdminWithMentors.Command>
{
    private readonly IGithubUserProvider _githubUserProvider;
    private readonly ILogger<SyncGithubAdminWithMentorsHandler> _logger;
    private readonly IOrganizationDetailsProvider _organizationDetailsProvider;
    private readonly IDatabaseContext _databaseContext;

    public SyncGithubAdminWithMentorsHandler(
        IDatabaseContext databaseContext,
        IOrganizationDetailsProvider organizationDetailsProvider,
        ILogger<SyncGithubAdminWithMentorsHandler> logger,
        IGithubUserProvider githubUserProvider)
    {
        _databaseContext = databaseContext;
        _organizationDetailsProvider = organizationDetailsProvider;
        _logger = logger;
        _githubUserProvider = githubUserProvider;
    }

    public async Task<Unit> Handle(SyncGithubAdminWithMentors.Command request, CancellationToken cancellationToken)
    {
        GithubSubjectCourseAssociation? courseGithub = await _databaseContext
            .SubjectCourseAssociations
            .OfType<GithubSubjectCourseAssociation>()
            .FirstOrDefaultAsync(a => a.GithubOrganizationName == request.OrganizationName, cancellationToken);

        if (courseGithub is null)
            throw new EntityNotFoundException($"Cannot found organization {request.OrganizationName} in database");

        IReadOnlyCollection<string> organizationOwners =
            await _organizationDetailsProvider.GetOrganizationOwners(courseGithub.GithubOrganizationName);
        HashSet<string> mentorUsernames = await GetMentorUsernames(courseGithub.SubjectCourse.Id, cancellationToken);

        var notMentorOwner = organizationOwners.Where(a => !mentorUsernames.Contains(a)).ToList();
        if (!notMentorOwner.Any())
        {
            _logger.LogWarning($"All github owners ({organizationOwners.Count}) already added as mentors.");
            return Unit.Value;
        }

        foreach (string owner in notMentorOwner)
        {
            _logger.LogInformation(
                $"Github user {owner} is owner and will be added as mentor to {courseGithub.SubjectCourse.Title}");
            await CreateMentorFromAdmin(courseGithub.SubjectCourse, owner, cancellationToken);
        }

        await _databaseContext.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }

    private async Task<HashSet<string>> GetMentorUsernames(Guid subjectCourseId, CancellationToken cancellationToken)
    {
        List<string> currentMentors = await _databaseContext
            .SubjectCourses
            .Where(s => s.Id == subjectCourseId)
            .SelectMany(a => a.Mentors)
            .SelectMany(m => m.User.Associations)
            .OfType<GithubUserAssociation>()
            .Select(a => a.GithubUsername)
            .ToListAsync(cancellationToken);

        return currentMentors.ToHashSet();
    }

    private async Task CreateMentorFromAdmin(
        SubjectCourse subjectCourse,
        string adminUsername,
        CancellationToken cancellationToken)
    {
        GithubUserAssociation? userAssociation = await _databaseContext
            .Users
            .SelectMany(u => u.Associations)
            .OfType<GithubUserAssociation>()
            .FirstOrDefaultAsync(a => a.GithubUsername == adminUsername, cancellationToken);

        if (userAssociation is not null)
        {
            subjectCourse.AddMentor(userAssociation.User);
        }
        else
        {
            bool isGithubUserExists = await _githubUserProvider.IsGithubUserExists(adminUsername);

            if (!isGithubUserExists)
                throw new DomainInvalidOperationException($"Github user with username {adminUsername} does not exist");

            var adminUser = new User(Guid.NewGuid(), adminUsername, adminUsername, adminUsername);
            var githubUserAssociation = new GithubUserAssociation(Guid.NewGuid(), adminUser, adminUsername);

            _databaseContext.Users.Add(adminUser);
            _databaseContext.UserAssociations.Add(githubUserAssociation);
            subjectCourse.AddMentor(adminUser);
        }
    }
}