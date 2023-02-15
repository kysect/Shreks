using ITMO.Dev.ASAP.Application.Dto.Querying;
using ITMO.Dev.ASAP.Application.Dto.Study;
using ITMO.Dev.ASAP.Application.Queries;
using ITMO.Dev.ASAP.Core.Study;
using ITMO.Dev.ASAP.DataAccess.Abstractions;
using ITMO.Dev.ASAP.Mapping.Mappings;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static ITMO.Dev.ASAP.Application.Contracts.Study.StudyGroups.Queries.FindGroupsByQuery;

namespace ITMO.Dev.ASAP.Application.Handlers.Study.StudyGroups;

internal class FindGroupsByQueryHandler : IRequestHandler<Query, Response>
{
    private readonly IDatabaseContext _context;
    private readonly IEntityQuery<StudentGroup, GroupQueryParameter> _query;

    public FindGroupsByQueryHandler(
        IEntityQuery<StudentGroup, GroupQueryParameter> query,
        IDatabaseContext context)
    {
        _query = query;
        _context = context;
    }

    public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
    {
        IQueryable<StudentGroup> query = _context.StudentGroups;
        query = _query.Apply(query, request.Configuration);

        List<StudentGroup> groups = await query.ToListAsync(cancellationToken);
        StudyGroupDto[] dto = groups.Select(x => x.ToDto()).ToArray();

        return new Response(dto);
    }
}