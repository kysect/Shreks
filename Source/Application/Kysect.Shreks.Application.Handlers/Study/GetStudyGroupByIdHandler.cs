using Kysect.Shreks.Core.Study;
using Kysect.Shreks.DataAccess.Abstractions;
using Kysect.Shreks.DataAccess.Abstractions.Extensions;
using Kysect.Shreks.Mapping.Mappings;
using MediatR;
using static Kysect.Shreks.Application.Contracts.Study.Queries.GetStudyGroupById;

namespace Kysect.Shreks.Application.Handlers.Study;

internal class GetStudyGroupByIdHandler : IRequestHandler<Query, Response>
{
    private readonly IShreksDatabaseContext _context;

    public GetStudyGroupByIdHandler(IShreksDatabaseContext context)
    {
        _context = context;
    }

    public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
    {
        StudentGroup studentGroup = await _context.StudentGroups
            .GetByIdAsync(request.Id, cancellationToken);

        return new Response(studentGroup.ToDto());
    }
}