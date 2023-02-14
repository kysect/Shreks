using ITMO.Dev.ASAP.Application.Dto.Study;
using ITMO.Dev.ASAP.Core.Study;
using ITMO.Dev.ASAP.DataAccess.Abstractions;
using ITMO.Dev.ASAP.Mapping.Mappings;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static ITMO.Dev.ASAP.Application.Contracts.Study.StudyGroups.Queries.GetStudyGroups;

namespace ITMO.Dev.ASAP.Application.Handlers.Study.StudyGroups;

internal class GetStudyGroupsHandler : IRequestHandler<Query, Response>
{
    private readonly IDatabaseContext _context;

    public GetStudyGroupsHandler(IDatabaseContext context)
    {
        _context = context;
    }

    public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
    {
        List<StudentGroup> studentGroups = await _context
            .StudentGroups
            .ToListAsync(cancellationToken);

        StudyGroupDto[] dto = studentGroups.Select(x => x.ToDto()).ToArray();
        return new Response(dto);
    }
}