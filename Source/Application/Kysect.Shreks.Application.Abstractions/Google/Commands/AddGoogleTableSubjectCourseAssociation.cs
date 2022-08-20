using MediatR;

namespace Kysect.Shreks.Application.Abstractions.Google.Commands;

public class AddGoogleTableSubjectCourseAssociation
{
    public record Command(Guid SubjectCourseId, string SpreadsheetId) : IRequest<Unit>;
}