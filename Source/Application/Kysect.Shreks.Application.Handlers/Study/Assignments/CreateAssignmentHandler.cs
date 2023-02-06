using Kysect.Shreks.Application.Contracts.Study.Assignments.Notifications;
using Kysect.Shreks.Application.Dto.Study;
using Kysect.Shreks.Core.Study;
using Kysect.Shreks.Core.ValueObject;
using Kysect.Shreks.DataAccess.Abstractions;
using Kysect.Shreks.DataAccess.Abstractions.Extensions;
using Kysect.Shreks.Mapping.Mappings;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static Kysect.Shreks.Application.Contracts.Study.Assignments.Commands.CreateAssignment;

namespace Kysect.Shreks.Application.Handlers.Study.Assignments;

internal class CreateAssignmentHandler : IRequestHandler<Command, Response>
{
    private readonly IShreksDatabaseContext _context;
    private readonly IPublisher _publisher;

    public CreateAssignmentHandler(IShreksDatabaseContext context, IPublisher publisher)
    {
        _context = context;
        _publisher = publisher;
    }

    public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
    {
        SubjectCourse subjectCourse = await _context.SubjectCourses
            .Include(x => x.Assignments)
            .Include(x => x.Groups)
            .GetByIdAsync(request.SubjectCourseId, cancellationToken);

        var assignment = new Assignment(
            Guid.NewGuid(),
            request.Title,
            request.ShortName,
            request.Order,
            new Points(request.MinPoints),
            new Points(request.MaxPoints),
            subjectCourse);

        _context.Assignments.Add(assignment);
        await _context.SaveChangesAsync(cancellationToken);

        AssignmentDto dto = assignment.ToDto();

        var notification = new AssignmentCreated.Notification(dto);
        await _publisher.PublishAsync(notification, cancellationToken);

        return new Response(dto);
    }
}