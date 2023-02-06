using Kysect.Shreks.Application.Dto.SubjectCourses;
using Kysect.Shreks.Core.Study;
using Kysect.Shreks.DataAccess.Abstractions;
using Kysect.Shreks.Mapping.Mappings;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static Kysect.Shreks.Application.Contracts.Study.SubjectCourses.Queries.GetSubjectCoursesBySubjectId;

namespace Kysect.Shreks.Application.Handlers.Study.SubjectCourses;

internal class GetSubjectCoursesBySubjectIdHandler : IRequestHandler<Query, Response>
{
    private readonly IShreksDatabaseContext _context;

    public GetSubjectCoursesBySubjectIdHandler(IShreksDatabaseContext context)
    {
        _context = context;
    }

    public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
    {
        List<SubjectCourse> courses = await _context.SubjectCourses
            .Where(x => x.Subject.Id.Equals(request.SubjectId))
            .ToListAsync(cancellationToken);

        SubjectCourseDto[] dto = courses
            .Select(x => x.ToDto())
            .ToArray();

        return new Response(dto);
    }
}