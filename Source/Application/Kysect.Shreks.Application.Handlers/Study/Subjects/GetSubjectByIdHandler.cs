using Kysect.Shreks.Core.Study;
using Kysect.Shreks.DataAccess.Abstractions;
using Kysect.Shreks.DataAccess.Abstractions.Extensions;
using Kysect.Shreks.Mapping.Mappings;
using MediatR;
using static Kysect.Shreks.Application.Contracts.Study.Subjects.Queries.GetSubjectById;

namespace Kysect.Shreks.Application.Handlers.Study.Subjects;

internal class GetSubjectByIdHandler : IRequestHandler<Query, Response>
{
    private readonly IShreksDatabaseContext _context;

    public GetSubjectByIdHandler(IShreksDatabaseContext context)
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