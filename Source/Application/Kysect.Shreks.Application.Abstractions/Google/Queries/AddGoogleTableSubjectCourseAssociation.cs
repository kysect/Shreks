using MediatR;

namespace Kysect.Shreks.Application.Abstractions.Google.Queries;

public class AddGoogleTableSubjectCourseAssociation
{
    public record Query(Guid SubjectCourseId, string SpreadsheetId) : IRequest<Unit>;
}