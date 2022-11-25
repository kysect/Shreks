using MediatR;

namespace Kysect.Shreks.Application.Contracts.Google.Commands;

public static class AddGoogleTableSubjectCourseAssociation
{
    public record Command(Guid SubjectCourseId, string SpreadsheetId) : IRequest<Unit>;
}