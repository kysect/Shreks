using Kysect.Shreks.Application.Dto.Study;
using Kysect.Shreks.Core.Study;
using Kysect.Shreks.DataAccess.Abstractions;
using Kysect.Shreks.Mapping.Mappings;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static Kysect.Shreks.Application.Contracts.Study.StudyGroups.Queries.GetStudyGroups;

namespace Kysect.Shreks.Application.Handlers.Study.StudyGroups;

internal class GetStudyGroupsHandler : IRequestHandler<Query, Response>
{
    private readonly IShreksDatabaseContext _context;

    public GetStudyGroupsHandler(IShreksDatabaseContext context)
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