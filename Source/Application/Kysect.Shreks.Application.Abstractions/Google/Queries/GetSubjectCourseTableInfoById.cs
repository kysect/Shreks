using MediatR;

namespace Kysect.Shreks.Application.Abstractions.Google.Queries;

public class GetSubjectCourseTableInfoById
{
    public record Query(Guid SubjectCourseId) : IRequest<Response>;

    public record Response(string Title, string? SpreadsheetId);
}