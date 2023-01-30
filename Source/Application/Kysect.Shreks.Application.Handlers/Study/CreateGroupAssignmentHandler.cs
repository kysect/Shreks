using Kysect.Shreks.Core.Study;
using Kysect.Shreks.DataAccess.Abstractions;
using Kysect.Shreks.DataAccess.Abstractions.Extensions;
using Kysect.Shreks.Mapping.Mappings;
using MediatR;
using static Kysect.Shreks.Application.Contracts.Study.Commands.CreateGroupAssignment;

namespace Kysect.Shreks.Application.Handlers.Study;

internal class CreateGroupAssignmentHandler : IRequestHandler<Command, Response>
{
    private readonly IShreksDatabaseContext _context;

    public CreateGroupAssignmentHandler(IShreksDatabaseContext context)
    {
        _context = context;
    }

    public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
    {
        StudentGroup studyGroup = await _context.StudentGroups.GetByIdAsync(request.GroupId, cancellationToken);
        Assignment assignment = await _context.Assignments.GetByIdAsync(request.AssignmentId, cancellationToken);

        var groupAssignment = new GroupAssignment(studyGroup, assignment, request.Deadline);
        _context.GroupAssignments.Add(groupAssignment);
        await _context.SaveChangesAsync(cancellationToken);

        return new Response(groupAssignment.ToDto());
    }
}