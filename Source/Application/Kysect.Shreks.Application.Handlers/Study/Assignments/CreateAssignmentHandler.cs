using Kysect.Shreks.Core.Study;
using Kysect.Shreks.Core.ValueObject;
using Kysect.Shreks.DataAccess.Abstractions;
using Kysect.Shreks.DataAccess.Abstractions.Extensions;
using Kysect.Shreks.Mapping.Mappings;
using MediatR;
using static Kysect.Shreks.Application.Contracts.Study.Commands.CreateAssignment;

namespace Kysect.Shreks.Application.Handlers.Study.Assignments;

internal class CreateAssignmentHandler : IRequestHandler<Command, Response>
{
    private readonly IShreksDatabaseContext _context;

    public CreateAssignmentHandler(IShreksDatabaseContext context)
    {
        _context = context;
    }

    public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
    {
        SubjectCourse subjectCourse = await _context.SubjectCourses
            .GetByIdAsync(request.SubjectCourseId, cancellationToken);

        var assignment = new Assignment(
            Guid.NewGuid(),
            request.Title,
            request.ShortName,
            request.Order,
            new Points(request.MinPoints),
            new Points(request.MaxPoints),
            subjectCourse);

        foreach (SubjectCourseGroup group in subjectCourse.Groups)
        {
            assignment.AddGroup(group.StudentGroup, DateOnly.FromDateTime(DateTime.UnixEpoch));
        }

        _context.Assignments.Add(assignment);
        await _context.SaveChangesAsync(cancellationToken);

        return new Response(assignment.ToDto());
    }
}