using MediatR;

namespace Kysect.Shreks.Application.Abstractions.Google.Notifications;

public record SubjectCoursePointsUpdatedNotification(Guid SubjectCourseId) : INotification;