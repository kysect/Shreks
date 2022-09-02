using AutoMapper;
using Kysect.Shreks.Application.Dto.Study;
using AutoMapper.QueryableExtensions;
using Kysect.Shreks.DataAccess.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static Kysect.Shreks.Application.Abstractions.Study.Queries.GetGroupAssignments;

namespace Kysect.Shreks.Application.Handlers.Study;

public class GetGroupAssignments : IRequestHandler<Query, Response>
{
    private readonly IShreksDatabaseContext _context;
    private readonly IMapper _mapper;

    public GetGroupAssignments(IShreksDatabaseContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
    {
        List<GroupAssignmentDto> subject = await _context
            .GroupAssignments
            .ProjectTo<GroupAssignmentDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

        return new Response(subject);
    }
}