using Kysect.Shreks.Application.Abstractions.Google;
using Kysect.Shreks.Application.Abstractions.Google.Models;
using Kysect.Shreks.Application.Abstractions.Google.Queries;
using Kysect.Shreks.Core.Study;
using Kysect.Shreks.Integration.Google.Sheets;
using Kysect.Shreks.Integration.Google.Tools;
using Kysect.Shreks.Integration.Google.Tools.Collection;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Kysect.Shreks.Integration.Google;

public class GoogleTableAccessor : IGoogleTableAccessor
{
    private static readonly TimeSpan DelayBetweenSheetUpdates = TimeSpan.FromMinutes(1);

    private static readonly ConcurrentHashSet<Guid> QueueUpdateSubjectCourseIds = new();
    private static readonly ConcurrentHashSet<Guid> PointsUpdateSubjectCourseIds = new();

    private static readonly SemaphoreSlim QueueUpdateSemaphore = new(1);
    private static readonly SemaphoreSlim PointsUpdateSemaphore = new(1);

    private static readonly SemaphoreSlim SpreadsheetCreationSemaphore = new(1);

    private readonly ISheet<CoursePoints> _pointsSheet;
    private readonly ISheet<StudentsQueue> _queueSheet;
    private readonly ISheetManagementService _sheetManagementService;
    private readonly IMediator _mediator;
    private readonly ILogger<GoogleTableAccessor> _logger;

    public GoogleTableAccessor(
        ISheet<CoursePoints> pointsSheet,
        ISheet<StudentsQueue> queueSheet,
        ISheetManagementService sheetManagementService,
        IMediator mediator,
        ILogger<GoogleTableAccessor> logger)
    {
        _pointsSheet = pointsSheet;
        _queueSheet = queueSheet;
        _sheetManagementService = sheetManagementService;
        _mediator = mediator;
        _logger = logger;
    }

    public async Task UpdatePointsAsync(Guid subjectCourseId, CancellationToken token = default)
    {
        PointsUpdateSubjectCourseIds.Add(subjectCourseId);

        await PointsUpdateSemaphore.WaitAsync(token);

        IReadOnlyCollection<Guid> subjectCourseIds = PointsUpdateSubjectCourseIds.GetAndClearValues();
        if (!subjectCourseIds.Any())
        {
            PointsUpdateSemaphore.Release();
            return;
        }
        
        foreach (var courseId in subjectCourseIds)
        {
            try
            {
                GetCoursePointsBySubjectCourse.Response response = await _mediator
                    .Send(new GetCoursePointsBySubjectCourse.Query(courseId), token);

                CoursePoints points = response.Points;

                string spreadsheetId = await GetSpreadsheetIdAsync(courseId, token);
                await _pointsSheet.UpdateAsync(spreadsheetId, points, token);

                _logger.LogInformation($"Successfully updated points sheet of course {courseId}.");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Failed to update points sheet of course {courseId}.");
            }
        }

        await Task.Delay(DelayBetweenSheetUpdates, token);
        PointsUpdateSemaphore.Release();
    }

    public async Task UpdateQueueAsync(Guid subjectCourseId, CancellationToken token = default)
    {
        QueueUpdateSubjectCourseIds.Add(subjectCourseId);
        
        await QueueUpdateSemaphore.WaitAsync(token);

        IReadOnlyCollection<Guid> subjectCourseIds = QueueUpdateSubjectCourseIds.GetAndClearValues();
        if (!subjectCourseIds.Any())
        {
            QueueUpdateSemaphore.Release();
            return;
        }
        
        foreach (var courseId in subjectCourseIds)
        {
            try
            {
                var queue = new StudentsQueue(Array.Empty<Submission>());

                //TODO: change to GetStudentQueueBySubjectCourse call

                string spreadsheetId = await GetSpreadsheetIdAsync(courseId, token);
                await _queueSheet.UpdateAsync(spreadsheetId, queue, token);

                _logger.LogInformation($"Successfully updated queue sheet of course {courseId}.");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Failed to update queue sheet of course {courseId}.");
            }
        }

        await Task.Delay(DelayBetweenSheetUpdates, token);
        QueueUpdateSemaphore.Release();
    }

    private async Task<string> GetSpreadsheetIdAsync(Guid subjectCourseId, CancellationToken token)
    {
        await SpreadsheetCreationSemaphore.WaitAsync(token);

        var response = await _mediator.Send(new GetGoogleTableSubjectCourseAssociation.Query(subjectCourseId), token);

        var googleTableAssociation = response.GoogleTableAssociation;

        if (googleTableAssociation is not null)
        {
            SpreadsheetCreationSemaphore.Release();
            return googleTableAssociation.SpreadsheetId;
        }

        string spreadsheetId = await _sheetManagementService.CreateSpreadsheetAsync(token);
        await _mediator.Send(new AddGoogleTableSubjectCourseAssociation.Query(subjectCourseId, spreadsheetId), token);

        SpreadsheetCreationSemaphore.Release();

        return spreadsheetId;
    }
}