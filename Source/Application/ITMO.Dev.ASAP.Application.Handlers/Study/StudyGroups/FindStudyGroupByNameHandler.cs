using ITMO.Dev.ASAP.Core.Study;
using ITMO.Dev.ASAP.DataAccess.Abstractions;
using ITMO.Dev.ASAP.Mapping.Mappings;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static ITMO.Dev.ASAP.Application.Contracts.Study.StudyGroups.Queries.FindStudyGroupByName;

namespace ITMO.Dev.ASAP.Application.Handlers.Study.StudyGroups;

internal class FindStudyGroupByNameHandler : IRequestHandler<Query, Response>
{
    private readonly IDatabaseContext _context;

    public FindStudyGroupByNameHandler(IDatabaseContext context)
    {
        _context = context;
    }

    public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
    {
        StudentGroup? studentGroup = await _context
            .StudentGroups
            .Where(g => g.Name == request.Name)
            .FirstOrDefaultAsync(cancellationToken);

        return new Response(studentGroup?.ToDto());
    }
}