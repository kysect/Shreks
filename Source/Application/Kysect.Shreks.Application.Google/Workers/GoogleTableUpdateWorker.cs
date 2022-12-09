using Kysect.Shreks.Application.Abstractions.Google.Notifications;
using Kysect.Shreks.Application.Google.Services;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Kysect.Shreks.Application.Google.Workers;

public class GoogleTableUpdateWorker : BackgroundService
{
    private static readonly TimeSpan DelayBetweenSheetUpdates = TimeSpan.FromSeconds(10);

    private readonly TableUpdateQueue _tableUpdateQueue;
    private readonly IServiceScopeFactory _serviceProvider;
    private readonly ILogger<GoogleTableUpdateWorker> _logger;
    private readonly Stopwatch _stopwatch;

    public GoogleTableUpdateWorker(
        TableUpdateQueue tableUpdateQueue,
        IServiceScopeFactory serviceProvider,
        ILogger<GoogleTableUpdateWorker> logger)
    {
        _tableUpdateQueue = tableUpdateQueue;
        _serviceProvider = serviceProvider;
        _logger = logger;

        _stopwatch = new Stopwatch();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // TODO: return to queue on fail
        // TODO: if something will went wrong loop is won't run any more until restart.
        using var timer = new PeriodicTimer(DelayBetweenSheetUpdates);

        while (stoppingToken.IsCancellationRequested is false && await timer.WaitForNextTickAsync(stoppingToken))
        {
            using IServiceScope serviceScope = _serviceProvider.CreateScope();
            IPublisher publisher = serviceScope.ServiceProvider.GetRequiredService<IPublisher>();

            _stopwatch.Restart();

            bool pointsTableUpdated = await UpdateTablePoints(publisher, stoppingToken);
            bool queueTableUpdated = await UpdateTableQueue(publisher, stoppingToken);

            _stopwatch.Stop();

            if (pointsTableUpdated || queueTableUpdated)
                _logger.LogInformation("Update tasks finished within {time} ms", _stopwatch.Elapsed.TotalMilliseconds);
        }
    }

    private async Task<bool> UpdateTablePoints(IPublisher publisher, CancellationToken cancellationToken)
    {
        IReadOnlyCollection<Guid> subjectCourses = _tableUpdateQueue.PointsUpdateSubjectCourseIds
            .GetAndClearValues();

        if (subjectCourses.Any())
            _logger.LogInformation("Going to update {count} subject courses points", subjectCourses.Count);

        foreach (Guid subjectCourseId in subjectCourses)
        {
            var notification = new SubjectCoursePointsUpdatedNotification(subjectCourseId);
            await publisher.Publish(notification, cancellationToken);
        }

        return subjectCourses.Any();
    }

    private async Task<bool> UpdateTableQueue(IPublisher publisher, CancellationToken cancellationToken)
    {
        IReadOnlyCollection<(Guid, Guid)> queues = _tableUpdateQueue
            .QueueUpdateSubjectCourseGroupIds
            .GetAndClearValues();

        if (queues.Any())
            _logger.LogInformation("Going to update {count} group queues", queues.Count);

        foreach ((Guid courseId, Guid groupId) in queues)
        {
            var notification = new SubjectCourseGroupQueueUpdatedNotification(courseId, groupId);
            await publisher.Publish(notification, cancellationToken);
        }

        return queues.Any();
    }
}