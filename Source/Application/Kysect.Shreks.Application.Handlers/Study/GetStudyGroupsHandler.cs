using AutoMapper;
using AutoMapper.QueryableExtensions;
using Kysect.Shreks.Application.Dto.Study;
using Kysect.Shreks.DataAccess.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static Kysect.Shreks.Application.Contracts.Study.Queries.GetStudyGroups;

namespace Kysect.Shreks.Application.Handlers.Study;

public class GetStudyGroupsHandler : IRequestHandler<Query, Response>
{
    private readonly IShreksDatabaseContext _context;
    private readonly IMapper _mapper;

    public GetStudyGroupsHandler(IShreksDatabaseContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
    {
        List<StudyGroupDto> studentGroups = await _context
            .StudentGroups
            .ProjectTo<StudyGroupDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken: cancellationToken);

        return new Response(studentGroups);
    }
}