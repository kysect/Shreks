using MediatR;

namespace Kysect.Shreks.Application.Abstractions.Google.Queries;

public class GetSpreadsheetIdBySubjectCourse
{
    public record Query(Guid SubjectCourseId) : IRequest<Response>;

    public record Response(string? SpreadsheetId);
}