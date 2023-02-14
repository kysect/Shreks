using ITMO.Dev.ASAP.Application.Dto.Study;
using MediatR;

namespace ITMO.Dev.ASAP.Application.Contracts.Study.StudyGroups.Notifications;

internal static class StudyGroupUpdated
{
    public record Notification(StudyGroupDto Group) : INotification;
}