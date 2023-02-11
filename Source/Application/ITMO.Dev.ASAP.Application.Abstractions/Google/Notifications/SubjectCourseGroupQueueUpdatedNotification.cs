using MediatR;

namespace ITMO.Dev.ASAP.Application.Abstractions.Google.Notifications;

public record SubjectCourseGroupQueueUpdatedNotification(Guid SubjectCourseId, Guid GroupId) : INotification;