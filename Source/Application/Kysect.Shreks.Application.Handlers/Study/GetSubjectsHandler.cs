using Kysect.Shreks.Application.Dto.Study;
using Kysect.Shreks.Core.Study;
using Kysect.Shreks.DataAccess.Abstractions;
using Kysect.Shreks.Mapping.Mappings;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static Kysect.Shreks.Application.Contracts.Study.Queries.GetSubjects;

namespace Kysect.Shreks.Application.Handlers.Study;

internal class GetSubjectsHandler : IRequestHandler<Query, Response>
{
    private readonly IShreksDatabaseContext _context;

    public GetSubjectsHandler(IShreksDatabaseContext context)
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