using ITMO.Dev.ASAP.Application.Dto.SubjectCourses;
using MediatR;

namespace ITMO.Dev.ASAP.Application.Contracts.Study.SubjectCourseGroups.Notifications;

internal static class SubjectCourseGroupCreated
{
    public record Notification(SubjectCourseGroupDto Group) : INotification;
}