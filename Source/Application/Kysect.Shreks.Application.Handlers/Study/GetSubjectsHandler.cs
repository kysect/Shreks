using AutoMapper;
using AutoMapper.QueryableExtensions;
using Kysect.Shreks.Application.Dto.Study;
using Kysect.Shreks.DataAccess.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static Kysect.Shreks.Application.Contracts.Study.Queries.GetSubjects;

namespace Kysect.Shreks.Application.Handlers.Study;

internal class GetSubjectsHandler : IRequestHandler<Query, Response>
{
    private readonly IShreksDatabaseContext _context;
    private readonly IMapper _mapper;

    public GetSubjectsHandler(IShreksDatabaseContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
    {
        List<SubjectDto> subject = await _context
            .Subjects
            .ProjectTo<SubjectDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken: cancellationToken);

        return new Response(subject);
    }
}