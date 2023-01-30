using Kysect.Shreks.Core.Study;
using Kysect.Shreks.DataAccess.Abstractions;
using Kysect.Shreks.DataAccess.Abstractions.Extensions;
using Kysect.Shreks.Mapping.Mappings;
using MediatR;
using static Kysect.Shreks.Application.Contracts.Study.Queries.GetSubjectCourseById;

namespace Kysect.Shreks.Application.Handlers.SubjectCourses;

internal class GetSubjectCourseByIdHandler : IRequestHandler<Query, Response>
{
    private readonly IShreksDatabaseContext _context;

    public GetSubjectCourseByIdHandler(IShreksDatabaseContext context)
    {
        _context = context;
    }

    public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
    {
        SubjectCourse subjectCourse = await _context
            .SubjectCourses
            .GetByIdAsync(request.Id, cancellationToken);

        return new Response(subjectCourse.ToDto());
    }
}