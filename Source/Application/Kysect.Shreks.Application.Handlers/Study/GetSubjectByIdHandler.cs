using AutoMapper;
using Kysect.Shreks.Application.Dto.Study;
using Kysect.Shreks.Core.Study;
using Kysect.Shreks.DataAccess.Abstractions;
using Kysect.Shreks.DataAccess.Abstractions.Extensions;
using MediatR;
using static Kysect.Shreks.Application.Contracts.Study.Queries.GetSubjectById;

namespace Kysect.Shreks.Application.Handlers.Study;

internal class GetSubjectByIdHandler : IRequestHandler<Query, Response>
{
    private readonly IShreksDatabaseContext _context;
    private readonly IMapper _mapper;

    public GetSubjectByIdHandler(IShreksDatabaseContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
    {
        Subject subject = await _context
            .Subjects
            .GetByIdAsync(request.Id, cancellationToken);

        return new Response(_mapper.Map<SubjectDto>(subject));
    }
}