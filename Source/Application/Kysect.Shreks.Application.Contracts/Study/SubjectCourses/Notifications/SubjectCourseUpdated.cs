using Kysect.Shreks.Application.Dto.SubjectCourses;
using MediatR;

namespace Kysect.Shreks.Application.Contracts.Study.SubjectCourses.Notifications;

internal static class SubjectCourseUpdated
{
    public record Notification(SubjectCourseDto SubjectCourse) : INotification;
}