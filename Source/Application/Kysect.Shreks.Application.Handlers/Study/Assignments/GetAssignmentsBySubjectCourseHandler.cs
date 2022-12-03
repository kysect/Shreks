using AutoMapper;
using Kysect.Shreks.Application.Dto.Study;
using Kysect.Shreks.Core.Study;
using Kysect.Shreks.DataAccess.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static Kysect.Shreks.Application.Contracts.Study.Queries.GetAssignmentsBySubjectCourse;

namespace Kysect.Shreks.Application.Handlers.Study.Assignments;

internal class GetAssignmentsBySubjectCourseHandler : IRequestHandler<Query, Response>
{
    private readonly IShreksDatabaseContext _context;
    private readonly IMapper _mapper;

    public GetAssignmentsBySubjectCourseHandler(IShreksDatabaseContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }


    public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
    {
        IReadOnlyCollection<Assignment> assignments = await _context
            .Assignments
            .Where(a => a.SubjectCourse.Id == request.SubjectCourseId)
            .ToListAsync(cancellationToken);

        AssignmentDto[] dto = assignments
            .Select(_mapper.Map<AssignmentDto>)
            .ToArray();

        return new Response(dto);
    }
}