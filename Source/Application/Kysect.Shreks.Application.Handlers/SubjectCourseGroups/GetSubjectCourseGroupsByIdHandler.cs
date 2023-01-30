using Kysect.Shreks.Core.Study;
using Kysect.Shreks.DataAccess.Abstractions;
using Kysect.Shreks.DataAccess.Abstractions.Extensions;
using Kysect.Shreks.Mapping.Mappings;
using MediatR;
using static Kysect.Shreks.Application.Contracts.Study.Queries.GetSubjectCourseGroupsBySubjectCourseId;

namespace Kysect.Shreks.Application.Handlers.SubjectCourseGroups;

internal class GetSubjectCourseGroupsByIdHandler : IRequestHandler<Query, Response>
{
    private readonly IShreksDatabaseContext _context;

    public GetSubjectCourseGroupsByIdHandler(IShreksDatabaseContext context)
    {
        _context = context;
    }

    public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
    {
        SubjectCourse subjectCourse =
            await _context.SubjectCourses.GetByIdAsync(request.SubjectCourseId, cancellationToken);

        var subjectCourseGroups = subjectCourse.Groups.Select(x => x.ToDto()).ToList();

        return new Response(subjectCourseGroups);
    }
}