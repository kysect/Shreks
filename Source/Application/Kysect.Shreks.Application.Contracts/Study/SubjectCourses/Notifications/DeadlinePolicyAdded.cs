using MediatR;

namespace Kysect.Shreks.Application.Contracts.Study.SubjectCourses.Notifications;

internal static class DeadlinePolicyAdded
{
    public record Notification(Guid SubjectCourseId) : INotification;
}