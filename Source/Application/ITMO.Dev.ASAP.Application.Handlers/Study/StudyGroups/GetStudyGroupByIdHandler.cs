using ITMO.Dev.ASAP.Core.Study;
using ITMO.Dev.ASAP.DataAccess.Abstractions;
using ITMO.Dev.ASAP.DataAccess.Abstractions.Extensions;
using ITMO.Dev.ASAP.Mapping.Mappings;
using MediatR;
using static ITMO.Dev.ASAP.Application.Contracts.Study.StudyGroups.Queries.GetStudyGroupById;

namespace ITMO.Dev.ASAP.Application.Handlers.Study.StudyGroups;

internal class GetStudyGroupByIdHandler : IRequestHandler<Query, Response>
{
    private readonly IDatabaseContext _context;

    public GetStudyGroupByIdHandler(IDatabaseContext context)
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