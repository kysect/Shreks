using Kysect.Shreks.Application.Dto.SubjectCourseAssociations;
using MediatR;

namespace Kysect.Shreks.Application.Abstractions.Github.Queries;

public static class GetSubjectCourseGithubAssociations
{
    public record Query : IRequest<Response>;

    public record Response(IReadOnlyCollection<GithubSubjectCourseAssociationDto> Associations);
}