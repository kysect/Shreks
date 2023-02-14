using ITMO.Dev.ASAP.Application.Dto.Study;
using MediatR;

namespace ITMO.Dev.ASAP.Application.Contracts.Study.Subjects.Notification;

internal static class SubjectUpdated
{
    public record Notification(SubjectDto Subject) : INotification;
}