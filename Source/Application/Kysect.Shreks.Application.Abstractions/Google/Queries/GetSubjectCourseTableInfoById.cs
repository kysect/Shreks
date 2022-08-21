using MediatR;

namespace Kysect.Shreks.Application.Abstractions.Google.Queries;

public static class GetSubjectCourseTableInfoById
{
    public record Query(Guid SubjectCourseId) : IRequest<Response>;

    public record Response(string SubjectCourseName, string? SpreadsheetId);
}