using Kysect.Shreks.Core.Study;
using Kysect.Shreks.DataAccess.Abstractions;
using Kysect.Shreks.Mapping.Mappings;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static Kysect.Shreks.Application.Contracts.Study.Queries.FindStudyGroupByName;

namespace Kysect.Shreks.Application.Handlers.Study;

internal class FindStudyGroupByNameHandler : IRequestHandler<Query, Response>
{
    private readonly IShreksDatabaseContext _context;

    public FindStudyGroupByNameHandler(IShreksDatabaseContext context)
    {
        _context = context;
    }

    public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
    {
        StudentGroup? studentGroup = await _context
            .StudentGroups
            .Where(g => g.Name == request.Name)
            .FirstOrDefaultAsync(cancellationToken);

        return new Response(studentGroup?.ToDto());
    }
}