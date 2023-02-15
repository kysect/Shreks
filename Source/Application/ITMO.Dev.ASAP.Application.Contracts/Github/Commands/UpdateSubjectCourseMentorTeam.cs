using MediatR;

namespace ITMO.Dev.ASAP.Application.Contracts.Github.Commands;

public static class UpdateSubjectCourseMentorTeam
{
    public record Command(Guid SubjectCourseId, string MentorsTeamName) : IRequest;
}