using Kysect.Shreks.Application.Abstractions.Google;
using Kysect.Shreks.Integration.Google.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Kysect.Shreks.Integration.Google;

public class GoogleTableUpdateWorker : BackgroundService, ITableUpdateQueue
{
    private static readonly TimeSpan DelayBetweenSheetUpdates = TimeSpan.FromMinutes(1);

    private readonly ConcurrentHashSet<Guid> _queueUpdateSubjectCourseIds;
    private readonly ConcurrentHashSet<Guid> _pointsUpdateSubjectCourseIds;

    private readonly IServiceScopeFactory _serviceProvider;
    
    public GoogleTableUpdateWorker(IServiceScopeFactory serviceProvider)
    {
        _serviceProvider = serviceProvider;

        _queueUpdateSubjectCourseIds = new ConcurrentHashSet<Guid>();
        _pointsUpdateSubjectCourseIds = new ConcurrentHashSet<Guid>();
    }

    protected override async Task ExecuteAsync(CancellationToken token)
    {
        using var timer = new PeriodicTimer(DelayBetweenSheetUpdates);
        while (!token.IsCancellationRequested && await timer.WaitForNextTickAsync(token))
        {
            using (IServiceScope serviceScope = _serviceProvider.CreateScope())
            {
                using (var googleTableAccessor = serviceScope.ServiceProvider.GetRequiredService<GoogleTableAccessor>())
                {
                    var pointsUpdateTasks = _pointsUpdateSubjectCourseIds
                        .GetAndClearValues()
                        .Select(i => googleTableAccessor.UpdatePointsAsync(i, token));

                    var queueUpdateTasks = _queueUpdateSubjectCourseIds
                        .GetAndClearValues()
                        .Select(i => googleTableAccessor.UpdateQueueAsync(i, token));

                    await Task.WhenAll(pointsUpdateTasks.Concat(queueUpdateTasks));
                }
            }
        }
    }

    public void EnqueueCoursePointsUpdate(Guid subjectCourseId)
        => _pointsUpdateSubjectCourseIds.Add(subjectCourseId);

    public void EnqueueSubmissionsQueueUpdate(Guid subjectCourseId)
        => _queueUpdateSubjectCourseIds.Add(subjectCourseId);
}