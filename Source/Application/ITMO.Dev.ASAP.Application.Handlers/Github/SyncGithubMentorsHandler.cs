using ITMO.Dev.ASAP.Application.Contracts.Github.Commands;
using ITMO.Dev.ASAP.Application.Contracts.Github.Notifications;
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

internal class SyncGithubMentorsHandler :
    IRequestHandler<SyncGithubMentors.Command>,
    INotificationHandler<SubjectCourseMentorTeamUpdated.Notification>
{
    private readonly IGithubUserProvider _githubUserProvider;
    private readonly ILogger<SyncGithubMentorsHandler> _logger;
    private readonly IOrganizationDetailsProvider _organizationDetailsProvider;
    private readonly IDatabaseContext _context;

    public SyncGithubMentorsHandler(
        IDatabaseContext context,
        IOrganizationDetailsProvider organizationDetailsProvider,
        ILogger<SyncGithubMentorsHandler> logger,
        IGithubUserProvider githubUserProvider)
    {
        _context = context;
        _organizationDetailsProvider = organizationDetailsProvider;
        _logger = logger;
        _githubUserProvider = githubUserProvider;
    }

    public async Task<Unit> Handle(SyncGithubMentors.Command request, CancellationToken cancellationToken)
    {
        GithubSubjectCourseAssociation? association = await _context
            .SubjectCourseAssociations
            .OfType<GithubSubjectCourseAssociation>()
            .Include(x => x.SubjectCourse)
            .Include(x => x.SubjectCourse.Mentors)
            .FirstOrDefaultAsync(a => a.GithubOrganizationName == request.OrganizationName, cancellationToken);

        if (association is null)
            throw new EntityNotFoundException($"Cannot found organization {request.OrganizationName}");

        await UpdateMentorsAsync(association, cancellationToken);

        return Unit.Value;
    }

    public async Task Handle(
        SubjectCourseMentorTeamUpdated.Notification notification,
        CancellationToken cancellationToken)
    {
        GithubSubjectCourseAssociation? association = await _context.SubjectCourses
            .Where(x => x.Id.Equals(notification.SubjectCourseId))
            .SelectMany(x => x.Associations)
            .OfType<GithubSubjectCourseAssociation>()
            .FirstOrDefaultAsync(cancellationToken);

        if (association is null)
            throw EntityNotFoundException.For<SubjectCourse>(notification.SubjectCourseId);

        await UpdateMentorsAsync(association, cancellationToken);
    }

    private async Task UpdateMentorsAsync(
        GithubSubjectCourseAssociation association,
        CancellationToken cancellationToken)
    {
        SubjectCourse subjectCourse = association.SubjectCourse;

        IReadOnlyCollection<string> mentorsTeamMembers = await _organizationDetailsProvider
            .GetOrganizationTeamMembers(association.GithubOrganizationName, association.MentorTeamName);

        IReadOnlyCollection<string> mentorUsernames = await GetMentorUsernames(
            association.SubjectCourse.Id,
            cancellationToken);

        string[] mentorsToAdd = mentorsTeamMembers.Except(mentorUsernames).ToArray();
        string[] mentorsToRemove = mentorUsernames.Except(mentorsTeamMembers).ToArray();

        await AddMentorsAsync(mentorsToAdd, subjectCourse, cancellationToken);
        await RemoveMentorsAsync(subjectCourse, mentorsToRemove, cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);
    }

    private async ValueTask AddMentorsAsync(
        IReadOnlyCollection<string> mentorsToAdd,
        SubjectCourse subjectCourse,
        CancellationToken cancellationToken)
    {
        if (mentorsToAdd.Count is 0)
        {
            _logger.LogInformation("All github mentors already added to course");
            return;
        }

        foreach (string mentorUsername in mentorsToAdd)
        {
            await AddGithubMentorAsync(subjectCourse, mentorUsername, cancellationToken);
        }
    }

    private async ValueTask RemoveMentorsAsync(
        SubjectCourse subjectCourse,
        IReadOnlyCollection<string> usernames,
        CancellationToken cancellationToken)
    {
        if (usernames.Count is 0)
            return;

        IQueryable<User> mentorUsers = _context.UserAssociations
            .OfType<GithubUserAssociation>()
            .Where(x => usernames.Contains(x.GithubUsername))
            .Select(x => x.User);

        List<Mentor> mentors = await _context.Mentors
            .Where(x => x.Course.Equals(subjectCourse))
            .Where(x => mentorUsers.Contains(x.User))
            .ToListAsync(cancellationToken);

        foreach (Mentor mentor in mentors)
        {
            subjectCourse.RemoveMentor(mentor);
        }
    }

    private async Task<IReadOnlyCollection<string>> GetMentorUsernames(
        Guid subjectCourseId,
        CancellationToken cancellationToken)
    {
        return await _context
            .SubjectCourses
            .Where(s => s.Id == subjectCourseId)
            .SelectMany(a => a.Mentors)
            .SelectMany(m => m.User.Associations)
            .OfType<GithubUserAssociation>()
            .Select(a => a.GithubUsername)
            .Distinct()
            .ToListAsync(cancellationToken);
    }

    private async Task AddGithubMentorAsync(
        SubjectCourse subjectCourse,
        string mentorUsername,
        CancellationToken cancellationToken)
    {
        GithubUserAssociation? userAssociation = await _context.Users
            .SelectMany(u => u.Associations)
            .OfType<GithubUserAssociation>()
            .Include(x => x.User)
            .FirstOrDefaultAsync(a => a.GithubUsername == mentorUsername, cancellationToken);

        if (userAssociation is null)
        {
            bool isGithubUserExists = await _githubUserProvider.IsGithubUserExists(mentorUsername);

            if (isGithubUserExists is false)
                throw new DomainInvalidOperationException($"Github user with username {mentorUsername} does not exist");

            var githubUser = new User(Guid.NewGuid(), mentorUsername, mentorUsername, mentorUsername);
            userAssociation = new GithubUserAssociation(Guid.NewGuid(), githubUser, mentorUsername);

            _context.Users.Add(githubUser);
            _context.UserAssociations.Add(userAssociation);
        }

        subjectCourse.AddMentor(userAssociation.User);
    }
}