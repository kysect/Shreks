using ITMO.Dev.ASAP.Core.Study;
using ITMO.Dev.ASAP.DataAccess.Abstractions;
using ITMO.Dev.ASAP.DataAccess.Abstractions.Extensions;
using ITMO.Dev.ASAP.Mapping.Mappings;
using MediatR;
using static ITMO.Dev.ASAP.Application.Contracts.Study.SubjectCourseGroups.Queries.GetSubjectCourseGroupsBySubjectCourseId;

namespace ITMO.Dev.ASAP.Application.Handlers.Study.SubjectCourseGroups;

internal class GetSubjectCourseGroupsByIdHandler : IRequestHandler<Query, Response>
{
    private readonly IDatabaseContext _context;

    public GetSubjectCourseGroupsByIdHandler(IDatabaseContext context)
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