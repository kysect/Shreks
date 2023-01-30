using Kysect.Shreks.Application.Dto.Querying;
using Kysect.Shreks.Application.Dto.Study;
using Kysect.Shreks.Application.Queries;
using Kysect.Shreks.Core.Study;
using Kysect.Shreks.DataAccess.Abstractions;
using Kysect.Shreks.Mapping.Mappings;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static Kysect.Shreks.Application.Contracts.StudyGroups.Queries.FindGroupsByQuery;

namespace Kysect.Shreks.Application.Handlers.StudyGroups;

internal class FindGroupsByQueryHandler : IRequestHandler<Query, Response>
{
    private readonly IShreksDatabaseContext _context;
    private readonly IEntityQuery<StudentGroup, GroupQueryParameter> _query;

    public FindGroupsByQueryHandler(
        IEntityQuery<StudentGroup, GroupQueryParameter> query,
        IShreksDatabaseContext context)
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