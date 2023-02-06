using Kysect.Shreks.Application.Abstractions.Google;
using Kysect.Shreks.Core.Study;
using Kysect.Shreks.Core.ValueObject;
using Kysect.Shreks.DataAccess.Abstractions;
using Kysect.Shreks.DataAccess.Abstractions.Extensions;
using Kysect.Shreks.Mapping.Mappings;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static Kysect.Shreks.Application.Contracts.Study.Commands.CreateAssignment;

namespace Kysect.Shreks.Application.Handlers.Study.Assignments;

internal class CreateAssignmentHandler : IRequestHandler<Command, Response>
{
    private readonly IShreksDatabaseContext _context;
    private readonly ITableUpdateQueue _tableUpdateQueue;

    public CreateAssignmentHandler(IShreksDatabaseContext context, ITableUpdateQueue tableUpdateQueue)
    {
        _context = context;
        _tableUpdateQueue = tableUpdateQueue;
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

        _tableUpdateQueue.EnqueueCoursePointsUpdate(subjectCourse.Id);

        return new Response(assignment.ToDto());
    }
}