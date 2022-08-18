using MediatR;

namespace Kysect.Shreks.Application.Abstractions.Google.Queries;

public class FindSpreadsheetIdBySubjectCourse
{
    public record Query(Guid SubjectCourseId) : IRequest<Response>;

    public record Response(string? SpreadsheetId);
}