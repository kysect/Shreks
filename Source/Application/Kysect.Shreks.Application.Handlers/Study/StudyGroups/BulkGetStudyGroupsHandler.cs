using AutoMapper;
using Kysect.Shreks.Application.Dto.Study;
using Kysect.Shreks.DataAccess.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static Kysect.Shreks.Application.Contracts.Study.Queries.BulkGetStudyGroups;

namespace Kysect.Shreks.Application.Handlers.Study.StudyGroups;

internal class BulkGetStudyGroupsHandler : IRequestHandler<Query, Response>
{
    private readonly IShreksDatabaseContext _context;
    private readonly IMapper _mapper;

    public BulkGetStudyGroupsHandler(IShreksDatabaseContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
    {
        var groups = await _context.StudentGroups
            .Where(x => request.Ids.Contains(x.Id))
            .ToListAsync(cancellationToken);

        var dto = groups.Select(_mapper.Map<StudyGroupDto>).ToArray();

        return new Response(dto);
    }
}