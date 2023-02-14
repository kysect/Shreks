using ITMO.Dev.ASAP.Application.Contracts.Github.Notifications;
using ITMO.Dev.ASAP.Common.Exceptions;
using ITMO.Dev.ASAP.Core.Study;
using ITMO.Dev.ASAP.Core.SubjectCourseAssociations;
using ITMO.Dev.ASAP.DataAccess.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static ITMO.Dev.ASAP.Application.Contracts.Github.Commands.UpdateSubjectCourseMentorTeam;

namespace ITMO.Dev.ASAP.Application.Handlers.Github.SubjectCourses;

internal class UpdateSubjectCourseMentorTeamHandler : IRequestHandler<Command>
{
    private readonly IDatabaseContext _context;
    private readonly IPublisher _publisher;

    public UpdateSubjectCourseMentorTeamHandler(IDatabaseContext context, IPublisher publisher)
    {
        _context = context;
        _publisher = publisher;
    }

    public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
    {
        GithubSubjectCourseAssociation? association = await _context.SubjectCourseAssociations
            .OfType<GithubSubjectCourseAssociation>()
            .FirstOrDefaultAsync(x => x.SubjectCourse.Id.Equals(request.SubjectCourseId), cancellationToken);

        if (association is null)
            throw EntityNotFoundException.For<SubjectCourse>(request.SubjectCourseId);

        association.MentorTeamName = request.MentorsTeamName;

        _context.SubjectCourseAssociations.Update(association);
        await _context.SaveChangesAsync(cancellationToken);

        var notification = new SubjectCourseMentorTeamUpdated.Notification(association.SubjectCourse.Id);
        await _publisher.PublishAsync(notification, cancellationToken);

        return Unit.Value;
    }
}