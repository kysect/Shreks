using MediatR;

namespace ITMO.Dev.ASAP.Application.Abstractions.Google.Notifications;

public record SubjectCoursePointsUpdatedNotification(Guid SubjectCourseId) : INotification;