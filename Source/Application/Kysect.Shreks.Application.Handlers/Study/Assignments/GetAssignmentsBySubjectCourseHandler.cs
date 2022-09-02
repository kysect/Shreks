using AutoMapper;
using AutoMapper.QueryableExtensions;
using Kysect.Shreks.Application.Dto.Study;
using Kysect.Shreks.DataAccess.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static Kysect.Shreks.Application.Abstractions.Study.Queries.GetAssignmentsBySubjectCourse;

namespace Kysect.Shreks.Application.Handlers.Study.Assignments;

public class GetAssignmentsBySubjectCourseHandler : IRequestHandler<Query, Response>
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
        var assignments = await _context
            .Assignments
            .Where(a => a.SubjectCourse.Id == request.SubjectCourseId)
            .ProjectTo<AssignmentDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

        return new Response(assignments);
    }
}