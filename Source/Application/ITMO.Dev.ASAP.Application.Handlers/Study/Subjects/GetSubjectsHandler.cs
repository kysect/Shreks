using ITMO.Dev.ASAP.Application.Dto.Study;
using ITMO.Dev.ASAP.Core.Study;
using ITMO.Dev.ASAP.DataAccess.Abstractions;
using ITMO.Dev.ASAP.Mapping.Mappings;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static ITMO.Dev.ASAP.Application.Contracts.Study.Subjects.Queries.GetSubjects;

namespace ITMO.Dev.ASAP.Application.Handlers.Study.Subjects;

internal class GetSubjectsHandler : IRequestHandler<Query, Response>
{
    private readonly IDatabaseContext _context;

    public GetSubjectsHandler(IDatabaseContext context)
    {
        _context = context;
    }

    public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
    {
        List<Subject> subject = await _context.Subjects.ToListAsync(cancellationToken);
        SubjectDto[] dto = subject.Select(x => x.ToDto()).ToArray();

        return new Response(dto);
    }
}