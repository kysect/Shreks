using Kysect.Shreks.Application.Dto.SubjectCourses;
using Kysect.Shreks.Core.Study;
using Kysect.Shreks.DataAccess.Abstractions;
using Kysect.Shreks.Mapping.Mappings;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static Kysect.Shreks.Application.Contracts.Study.Queries.GetSubjectCourses;

namespace Kysect.Shreks.Application.Handlers.SubjectCourses;

internal class GetSubjectCoursesHandler : IRequestHandler<Query, Response>
{
    private readonly IShreksDatabaseContext _context;

    public GetSubjectCoursesHandler(IShreksDatabaseContext context)
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