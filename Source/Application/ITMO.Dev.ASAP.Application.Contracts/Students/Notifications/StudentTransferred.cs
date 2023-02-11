using MediatR;

namespace ITMO.Dev.ASAP.Application.Contracts.Students.Notifications;

internal static class StudentTransferred
{
    public record Notification(Guid StudentId, Guid NewGroupId, Guid? OldGroupId) : INotification;
}