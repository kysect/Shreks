﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Kysect.Shreks.Application.TableManagement;

public class GoogleTableUpdateWorker : BackgroundService
{
    private static readonly TimeSpan DelayBetweenSheetUpdates = TimeSpan.FromSeconds(10);

    private readonly TableUpdateQueue _tableUpdateQueue;
    private readonly IServiceScopeFactory _serviceProvider;
    private readonly ILogger<GoogleTableUpdateWorker> _logger;
    private readonly Stopwatch _stopwatch;

    public GoogleTableUpdateWorker(TableUpdateQueue tableUpdateQueue, IServiceScopeFactory serviceProvider, ILogger<GoogleTableUpdateWorker> logger)
    {
        _tableUpdateQueue = tableUpdateQueue;
        _serviceProvider = serviceProvider;
        _logger = logger;

        _stopwatch = new Stopwatch();
    }

    protected override async Task ExecuteAsync(CancellationToken token)
    {
        // TODO: return to queue on fail
        // TODO: if something will went wrong loop is won't run any more until restart.
        using var timer = new PeriodicTimer(DelayBetweenSheetUpdates);
        while (!token.IsCancellationRequested && await timer.WaitForNextTickAsync(token))
        {
            using IServiceScope serviceScope = _serviceProvider.CreateScope();
            using var googleTableAccessor = serviceScope.ServiceProvider.GetRequiredService<ITableAccessor>();

            _stopwatch.Restart();

            var pointsTableUpdated = await UpdateTablePoints(googleTableAccessor, token);
            var queueTableUpdated = await UpdateTableQueue(googleTableAccessor, token);

            _stopwatch.Stop();

            if (pointsTableUpdated || queueTableUpdated)
                _logger.LogInformation("Update tasks finished within {time} ms", _stopwatch.Elapsed.TotalMilliseconds);
        }
    }

    private async Task<bool> UpdateTablePoints(ITableAccessor googleTableAccessor, CancellationToken token)
    {
        IReadOnlyCollection<Guid> points = _tableUpdateQueue
            .PointsUpdateSubjectCourseIds
            .GetAndClearValues();

        if (points.Any())
            _logger.LogInformation("Going to update {count} subject courses points", points.Count);

        foreach (Guid point in points)
            await googleTableAccessor.UpdatePointsAsync(point, token);

        return points.Any();
    }

    private async Task<bool> UpdateTableQueue(ITableAccessor googleTableAccessor, CancellationToken token)
    {
        IReadOnlyCollection<(Guid, Guid)> queues = _tableUpdateQueue
            .QueueUpdateSubjectCourseGroupIds
            .GetAndClearValues();

        if (queues.Any())
            _logger.LogInformation("Going to update {count} group queues", queues.Count);

        foreach ((Guid courseId, Guid groupId) in queues)
            await googleTableAccessor.UpdateQueueAsync(courseId, groupId, token);

        return queues.Any();
    }
}