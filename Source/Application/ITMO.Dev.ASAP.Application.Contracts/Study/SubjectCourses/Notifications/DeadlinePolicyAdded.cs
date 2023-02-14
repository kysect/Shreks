using MediatR;

namespace ITMO.Dev.ASAP.Application.Contracts.Study.SubjectCourses.Notifications;

internal static class DeadlinePolicyAdded
{
    public record Notification(Guid SubjectCourseId) : INotification;
}