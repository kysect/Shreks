using ITMO.Dev.ASAP.Application.Dto.SubjectCourses;
using MediatR;

namespace ITMO.Dev.ASAP.Application.Contracts.Study.SubjectCourses.Notifications;

internal static class SubjectCourseUpdated
{
    public record Notification(SubjectCourseDto SubjectCourse) : INotification;
}