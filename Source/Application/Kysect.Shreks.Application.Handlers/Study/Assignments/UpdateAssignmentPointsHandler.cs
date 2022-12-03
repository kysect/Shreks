using AutoMapper;
using Kysect.Shreks.Application.Dto.Study;
using Kysect.Shreks.Core.ValueObject;
using Kysect.Shreks.DataAccess.Abstractions;
using Kysect.Shreks.DataAccess.Abstractions.Extensions;
using MediatR;

using static Kysect.Shreks.Application.Contracts.Study.Commands.UpdateAssignmentPoints;

namespace Kysect.Shreks.Application.Handlers.Study.Assignments;

internal class UpdateAssignmentPointsHandler : IRequestHandler<Command, Response>
{
    private readonly IShreksDatabaseContext _context;
    private readonly IMapper _mapper;

    public UpdateAssignmentPointsHandler(IShreksDatabaseContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
    {
        var assignment = await _context.Assignments.GetByIdAsync(request.AssignmentId, cancellationToken);

        assignment.UpdateMinPoints(new Points(request.MinPoints));
        assignment.UpdateMaxPoints(new Points(request.MaxPoints));

        _context.Assignments.Update(assignment);
        await _context.SaveChangesAsync(cancellationToken);

        var dto = _mapper.Map<AssignmentDto>(assignment);

        return new Response(dto);
    }
}