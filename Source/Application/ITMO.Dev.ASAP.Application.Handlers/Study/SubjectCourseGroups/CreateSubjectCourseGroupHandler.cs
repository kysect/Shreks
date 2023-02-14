using ITMO.Dev.ASAP.Application.Contracts.Study.SubjectCourseGroups.Notifications;
using ITMO.Dev.ASAP.Application.Dto.SubjectCourses;
using ITMO.Dev.ASAP.Core.Study;
using ITMO.Dev.ASAP.DataAccess.Abstractions;
using ITMO.Dev.ASAP.DataAccess.Abstractions.Extensions;
using ITMO.Dev.ASAP.Mapping.Mappings;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static ITMO.Dev.ASAP.Application.Contracts.Study.SubjectCourseGroups.Commands.CreateSubjectCourseGroup;

namespace ITMO.Dev.ASAP.Application.Handlers.Study.SubjectCourseGroups;

internal class CreateSubjectCourseGroupHandler : IRequestHandler<Command, Response>
{
    private readonly IDatabaseContext _context;
    private readonly IPublisher _publisher;

    public CreateSubjectCourseGroupHandler(IDatabaseContext context, IPublisher publisher)
    {
        _context = context;
        _publisher = publisher;
    }

    public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
    {
        SubjectCourse subjectCourse = await _context.SubjectCourses
            .Include(x => x.Assignments)
            .ThenInclude(x => x.GroupAssignments)
            .GetByIdAsync(request.SubjectCourseId, cancellationToken);

        StudentGroup studentGroup = await _context.StudentGroups
            .GetByIdAsync(request.StudentGroupId, cancellationToken);

        SubjectCourseGroup group = subjectCourse.AddGroup(studentGroup);

        await _context.SubjectCourseGroups.AddAsync(group, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        SubjectCourseGroupDto dto = group.ToDto();

        var notification = new SubjectCourseGroupCreated.Notification(dto);
        await _publisher.PublishAsync(notification, cancellationToken);

        return new Response(dto);
    }
}