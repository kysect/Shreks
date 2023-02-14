using MediatR;

namespace ITMO.Dev.ASAP.Application.Contracts.Study.SubjectCourseGroups.Notifications;

internal static class SubjectCourseGroupDeleted
{
    public record Notification(Guid SubjectCourseId, Guid GroupId) : INotification;
}