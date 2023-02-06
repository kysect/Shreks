using Kysect.Shreks.Application.Dto.Study;
using Kysect.Shreks.Core.Study;
using Kysect.Shreks.DataAccess.Abstractions;
using Kysect.Shreks.Mapping.Mappings;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static Kysect.Shreks.Application.Contracts.Study.StudyGroups.Queries.BulkGetStudyGroups;

namespace Kysect.Shreks.Application.Handlers.Study.StudyGroups;

internal class BulkGetStudyGroupsHandler : IRequestHandler<Query, Response>
{
    private readonly IShreksDatabaseContext _context;

    public BulkGetStudyGroupsHandler(IShreksDatabaseContext context)
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