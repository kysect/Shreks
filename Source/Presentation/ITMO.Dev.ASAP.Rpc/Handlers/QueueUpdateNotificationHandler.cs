using ITMO.Dev.ASAP.Application.Contracts.Study.Queues.Notifications;
using ITMO.Dev.ASAP.Application.Dto.Tables;
using ITMO.Dev.ASAP.Rpc.Hubs;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace ITMO.Dev.ASAP.Rpc.Handlers;

internal class QueueUpdateNotificationHandler : INotificationHandler<SubmissionsQueueUpdated.Notification>
{
    private readonly IHubContext<QueueHub> _hubContext;

    public QueueUpdateNotificationHandler(IHubContext<QueueHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task Handle(SubmissionsQueueUpdated.Notification notification, CancellationToken cancellationToken)
    {
        SubmissionsQueueDto submissionsQueue = notification.SubmissionsQueue;
        await _hubContext.Clients.All.SendAsync("UpdateQueue", submissionsQueue, cancellationToken);
    }
}