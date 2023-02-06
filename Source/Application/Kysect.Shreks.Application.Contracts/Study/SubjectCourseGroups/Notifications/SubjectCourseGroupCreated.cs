using Kysect.Shreks.Application.Dto.SubjectCourses;
using MediatR;

namespace Kysect.Shreks.Application.Contracts.Study.SubjectCourseGroups.Notifications;

public static class SubjectCourseGroupCreated
{
    public record Notification(SubjectCourseGroupDto Group) : INotification;
}