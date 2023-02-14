using ITMO.Dev.ASAP.Application.Dto.SubjectCourses;
using ITMO.Dev.ASAP.Core.Study;
using ITMO.Dev.ASAP.DataAccess.Abstractions;
using ITMO.Dev.ASAP.Mapping.Mappings;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static ITMO.Dev.ASAP.Application.Contracts.Study.SubjectCourses.Queries.GetSubjectCourses;

namespace ITMO.Dev.ASAP.Application.Handlers.Study.SubjectCourses;

internal class GetSubjectCoursesHandler : IRequestHandler<Query, Response>
{
    private readonly IDatabaseContext _context;

    public GetSubjectCoursesHandler(IDatabaseContext context)
    {
        _context = context;
    }

    public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
    {
        List<SubjectCourse> subjectCourse = await _context
            .SubjectCourses
            .ToListAsync(cancellationToken);

        SubjectCourseDto[] dto = subjectCourse
            .Select(x => x.ToDto())
            .ToArray();

        return new Response(dto);
    }
}