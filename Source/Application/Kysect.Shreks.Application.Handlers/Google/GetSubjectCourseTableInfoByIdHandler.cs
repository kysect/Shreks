using Kysect.Shreks.Core.Study;
using Kysect.Shreks.Core.SubjectCourseAssociations;
using Kysect.Shreks.DataAccess.Abstractions;
using Kysect.Shreks.DataAccess.Abstractions.Extensions;
using MediatR;
using static Kysect.Shreks.Application.Abstractions.Google.Queries.GetSubjectCourseTableInfoById;

namespace Kysect.Shreks.Application.Handlers.Google;

public class GetSubjectCourseTableInfoByIdHandler : IRequestHandler<Query, Response>
{
    private readonly IShreksDatabaseContext _context;

    public GetSubjectCourseTableInfoByIdHandler(IShreksDatabaseContext context)
    {
        _context = context;
    }

    public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
    {
        SubjectCourse course = await _context.SubjectCourses.GetByIdAsync(request.SubjectCourseId, cancellationToken);

        var tableAssociation = course.Associations
            .OfType<GoogleTableSubjectCourseAssociation>()
            .FirstOrDefault();

        return new Response(course.Name, tableAssociation?.SpreadsheetId);
    }
}