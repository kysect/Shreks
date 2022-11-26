using AutoMapper;
using Kysect.Shreks.Application.Dto.Study;
using Kysect.Shreks.Core.Study;
using Kysect.Shreks.DataAccess.Abstractions;
using Kysect.Shreks.DataAccess.Abstractions.Extensions;
using MediatR;
using static Kysect.Shreks.Application.Contracts.Study.Queries.GetStudyGroupById;

namespace Kysect.Shreks.Application.Handlers.Study;

internal class GetStudyGroupByIdHandler : IRequestHandler<Query, Response>
{
    private readonly IShreksDatabaseContext _context;
    private readonly IMapper _mapper;

    public GetStudyGroupByIdHandler(IShreksDatabaseContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
    {
        StudentGroup studentGroup = await _context
            .StudentGroups
            .GetByIdAsync(request.Id, cancellationToken: cancellationToken);

        return new Response(_mapper.Map<StudyGroupDto>(studentGroup));
    }
}