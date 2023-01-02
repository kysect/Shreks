using Kysect.Shreks.Application.Contracts.Github.Commands;
using Kysect.Shreks.Application.GithubWorkflow.Abstractions.Providers;
using Kysect.Shreks.Common.Exceptions;
using Kysect.Shreks.Core.SubjectCourseAssociations;
using Kysect.Shreks.Core.UserAssociations;
using Kysect.Shreks.DataAccess.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Kysect.Shreks.Core.Study;
using Microsoft.Extensions.Logging;

namespace Kysect.Shreks.Application.Handlers.Github;

internal class SyncGithubAdminWithMentorsHandler : IRequestHandler<SyncGithubAdminWithMentors.Command>
{
    private readonly IShreksDatabaseContext _shreksDatabaseContext;
    private readonly IOrganizationDetailsProvider _organizationDetailsProvider;
    private readonly ILogger<SyncGithubAdminWithMentorsHandler> _logger;
    private readonly IGithubUserProvider _githubUserProvider;

    public SyncGithubAdminWithMentorsHandler(
        IShreksDatabaseContext shreksDatabaseContext,
        IOrganizationDetailsProvider organizationDetailsProvider,
        ILogger<SyncGithubAdminWithMentorsHandler> logger,
        IGithubUserProvider githubUserProvider)
    {
        _shreksDatabaseContext = shreksDatabaseContext;
        _organizationDetailsProvider = organizationDetailsProvider;
        _logger = logger;
        _githubUserProvider = githubUserProvider;
    }

    public async Task<Unit> Handle(SyncGithubAdminWithMentors.Command request, CancellationToken cancellationToken)
    {
        GithubSubjectCourseAssociation? courseGithub = await _shreksDatabaseContext
            .SubjectCourseAssociations
            .OfType<GithubSubjectCourseAssociation>()
            .FirstOrDefaultAsync(a => a.GithubOrganizationName == request.OrganizationName, cancellationToken);

        if (courseGithub is null)
            throw new EntityNotFoundException($"Cannot found organization {request.OrganizationName} in database");

        IReadOnlyCollection<string> organizationOwners = await _organizationDetailsProvider.GetOrganizationOwners(courseGithub.GithubOrganizationName);
        HashSet<string> mentorUsernames = await GetMentorUsernames(courseGithub.SubjectCourse.Id, cancellationToken);

        List<string> notMentorOwner = organizationOwners.Where(a => !mentorUsernames.Contains(a)).ToList();
        if (!notMentorOwner.Any())
        {
            _logger.LogWarning($"All github owners ({organizationOwners.Count}) already added as mentors.");
            return Unit.Value;
        }

        foreach (string owner in notMentorOwner)
        {
            _logger.LogInformation($"Github user {owner} is owner and will be added as mentor to {courseGithub.SubjectCourse.Title}");
            await CreateMentorFromAdmin(courseGithub.SubjectCourse, owner, cancellationToken);
        }

        await _shreksDatabaseContext.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }

    private async Task<HashSet<string>> GetMentorUsernames(Guid subjectCourseId, CancellationToken cancellationToken)
    {
        List<string> currentMentors = await _shreksDatabaseContext
            .SubjectCourses
            .Where(s => s.Id == subjectCourseId)
            .SelectMany(a => a.Mentors)
            .SelectMany(m => m.User.Associations)
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
            .FirstOrDefaultAsync(a => a.GithubUsername == adminUsername, cancellationToken);

        if (userAssociation is not null)
        {
            subjectCourse.AddMentor(userAssociation.User);
        }
        else
        {
            Boolean isGithubUserExists = await _githubUserProvider.IsGithubUserExists(adminUsername);

            if (!isGithubUserExists)
                throw new DomainInvalidOperationException($"Github user with username {adminUsername} does not exist");

            var adminUser = new Core.Users.User(adminUsername, adminUsername, adminUsername);
            var githubUserAssociation = new GithubUserAssociation(adminUser, adminUsername);

            _shreksDatabaseContext.Users.Add(adminUser);
            _shreksDatabaseContext.UserAssociations.Add(githubUserAssociation);
            subjectCourse.AddMentor(adminUser);
        }
    }
}