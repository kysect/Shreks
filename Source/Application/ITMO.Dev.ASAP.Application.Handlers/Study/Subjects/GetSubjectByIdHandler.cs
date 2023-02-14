using ITMO.Dev.ASAP.Core.Study;
using ITMO.Dev.ASAP.DataAccess.Abstractions;
using ITMO.Dev.ASAP.DataAccess.Abstractions.Extensions;
using ITMO.Dev.ASAP.Mapping.Mappings;
using MediatR;
using static ITMO.Dev.ASAP.Application.Contracts.Study.Subjects.Queries.GetSubjectById;

namespace ITMO.Dev.ASAP.Application.Handlers.Study.Subjects;

internal class GetSubjectByIdHandler : IRequestHandler<Query, Response>
{
    private readonly IDatabaseContext _context;

    public GetSubjectByIdHandler(IDatabaseContext context)
    {
        _context = context;
    }

    public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
    {
        Subject subject = await _context
            .Subjects
            .GetByIdAsync(request.Id, cancellationToken);

        return new Response(subject.ToDto());
    }
}