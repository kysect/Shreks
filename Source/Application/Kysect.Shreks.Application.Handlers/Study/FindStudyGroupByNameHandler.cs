using AutoMapper;
using Kysect.Shreks.Application.Dto.Study;
using Kysect.Shreks.Core.Study;
using Kysect.Shreks.DataAccess.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static Kysect.Shreks.Application.Contracts.Study.Queries.FindStudyGroupByName;

namespace Kysect.Shreks.Application.Handlers.Study;

internal class FindStudyGroupByNameHandler : IRequestHandler<Query, Response>
{
    private readonly IShreksDatabaseContext _context;
    private readonly IMapper _mapper;

    public FindStudyGroupByNameHandler(IShreksDatabaseContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
    {
        StudentGroup? studentGroup = await _context
            .StudentGroups
            .Where(g => g.Name == request.Name)
            .FirstOrDefaultAsync(cancellationToken);

        if (studentGroup is null)
            return new Response(null);

        return new Response(_mapper.Map<StudyGroupDto>(studentGroup));
    }
}