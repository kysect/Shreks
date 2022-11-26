using MediatR;

namespace Kysect.Shreks.Application.Abstractions.Google.Notifications;

public record SubjectCourseGroupQueueUpdatedNotification(Guid SubjectCourseId, Guid GroupId) : INotification;