using Kysect.Shreks.Application.Dto.SubjectCourses;
using MediatR;

namespace Kysect.Shreks.Application.Contracts.Study.SubjectCourseGroups.Notifications;

internal static class SubjectCourseGroupCreated
{
    public record Notification(SubjectCourseGroupDto Group) : INotification;
}