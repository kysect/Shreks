using ITMO.Dev.ASAP.Application.Dto.Study;
using ITMO.Dev.ASAP.Core.Study;
using ITMO.Dev.ASAP.DataAccess.Abstractions;
using ITMO.Dev.ASAP.Mapping.Mappings;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static ITMO.Dev.ASAP.Application.Contracts.Study.StudyGroups.Queries.BulkGetStudyGroups;

namespace ITMO.Dev.ASAP.Application.Handlers.Study.StudyGroups;

internal class BulkGetStudyGroupsHandler : IRequestHandler<Query, Response>
{
    private readonly IDatabaseContext _context;

    public BulkGetStudyGroupsHandler(IDatabaseContext context)
    {
        _context = context;
    }

    public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
    {
        List<StudentGroup> groups = await _context.StudentGroups
            .Where(x => request.Ids.Contains(x.Id))
            .ToListAsync(cancellationToken);

        StudyGroupDto[] dto = groups.Select(x => x.ToDto()).ToArray();

        return new Response(dto);
    }
}