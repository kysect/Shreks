using MediatR;

namespace ITMO.Dev.ASAP.Application.Contracts.Github.Notifications;

internal static class SubjectCourseMentorTeamUpdated
{
    public record Notification(Guid SubjectCourseId) : INotification;
}