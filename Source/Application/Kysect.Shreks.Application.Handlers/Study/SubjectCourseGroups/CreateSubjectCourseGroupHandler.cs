using Kysect.Shreks.Application.Contracts.Study.SubjectCourseGroups.Notifications;
using Kysect.Shreks.Application.Dto.SubjectCourses;
using Kysect.Shreks.Core.Study;
using Kysect.Shreks.DataAccess.Abstractions;
using Kysect.Shreks.DataAccess.Abstractions.Extensions;
using Kysect.Shreks.Mapping.Mappings;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static Kysect.Shreks.Application.Contracts.Study.SubjectCourseGroups.Commands.CreateSubjectCourseGroup;

namespace Kysect.Shreks.Application.Handlers.Study.SubjectCourseGroups;

internal class CreateSubjectCourseGroupHandler : IRequestHandler<Command, Response>
{
    private readonly IShreksDatabaseContext _context;
    private readonly IPublisher _publisher;

    public CreateSubjectCourseGroupHandler(IShreksDatabaseContext context, IPublisher publisher)
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
        await _publisher.Publish(notification, cancellationToken);

        return new Response(dto);
    }
}