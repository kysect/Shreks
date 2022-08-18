using Kysect.Shreks.Core.SubjectCourseAssociations;
using MediatR;

namespace Kysect.Shreks.Application.Abstractions.Google.Queries;

public class GetGoogleTableSubjectCourseAssociation
{
    public record Query(Guid SubjectCourseId) : IRequest<Response>;

    public record Response(GoogleTableSubjectCourseAssociation? GoogleTableAssociation);
}