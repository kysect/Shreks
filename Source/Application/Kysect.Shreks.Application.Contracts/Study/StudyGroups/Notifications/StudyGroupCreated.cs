using Kysect.Shreks.Application.Dto.Study;
using MediatR;

namespace Kysect.Shreks.Application.Contracts.Study.StudyGroups.Notifications;

internal static class StudyGroupCreated
{
    public record Notification(StudyGroupDto Group) : INotification;
}