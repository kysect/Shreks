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

    private readonly ConcurrentHashSet<Guid> _queueUpdateSubjectCourseIds;
    private readonly ConcurrentHashSet<Guid> _pointsUpdateSubjectCourseIds;

    private readonly SemaphoreSlim _queueUpdateSemaphore;
    private readonly SemaphoreSlim _pointsUpdateSemaphore;

    private readonly SemaphoreSlim _spreadsheetCreationSemaphore;

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

        _queueUpdateSubjectCourseIds = new ConcurrentHashSet<Guid>();
        _pointsUpdateSubjectCourseIds = new ConcurrentHashSet<Guid>();
        _queueUpdateSemaphore = new SemaphoreSlim(1, 1);
        _pointsUpdateSemaphore = new SemaphoreSlim(1, 1);
        _spreadsheetCreationSemaphore = new SemaphoreSlim(1, 1);
    }

    public async Task UpdatePointsAsync(Guid subjectCourseId, CancellationToken token = default)
    {
        _pointsUpdateSubjectCourseIds.Add(subjectCourseId);

        await _pointsUpdateSemaphore.WaitAsync(token);

        var subjectCourseIds = _pointsUpdateSubjectCourseIds.GetAndClearValues();

        if (!subjectCourseIds.Any())
        {
            _pointsUpdateSemaphore.Release();
            return;
        }
        
        foreach (var courseId in subjectCourseIds)
        {
            try
            {
                var query = new GetCoursePointsBySubjectCourse.Query(courseId);
                var response = await _mediator.Send(query, token);

                var points = response.Points;
                var spreadsheetId = await GetSpreadsheetIdAsync(courseId, token);
                await _pointsSheet.UpdateAsync(spreadsheetId, points, token);

                _logger.LogInformation("Successfully updated points sheet of course {CourseId}.", courseId);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to update points sheet of course {CourseId}.", courseId);
            }
        }

        await Task.Delay(DelayBetweenSheetUpdates, token);
        _pointsUpdateSemaphore.Release();
    }

    public async Task UpdateQueueAsync(Guid subjectCourseId, CancellationToken token = default)
    {
        _queueUpdateSubjectCourseIds.Add(subjectCourseId);
        
        await _queueUpdateSemaphore.WaitAsync(token);

        var subjectCourseIds = _queueUpdateSubjectCourseIds.GetAndClearValues();

        if (!subjectCourseIds.Any())
        {
            _queueUpdateSemaphore.Release();
            return;
        }
        
        foreach (var courseId in subjectCourseIds)
        {
            try
            {
                var queue = new StudentsQueue(Array.Empty<Submission>());

                //TODO: change to GetStudentQueueBySubjectCourse call

                var spreadsheetId = await GetSpreadsheetIdAsync(courseId, token);
                await _queueSheet.UpdateAsync(spreadsheetId, queue, token);

                _logger.LogInformation("Successfully updated queue sheet of course {CourseId}.", courseId);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to update queue sheet of course {CourseId}.", courseId);
            }
        }

        await Task.Delay(DelayBetweenSheetUpdates, token);
        _queueUpdateSemaphore.Release();
    }

    private async Task<string> GetSpreadsheetIdAsync(Guid subjectCourseId, CancellationToken token)
    {
        await _spreadsheetCreationSemaphore.WaitAsync(token);

        var response = await _mediator.Send(new GetSpreadsheetIdBySubjectCourse.Query(subjectCourseId), token);
        
        if (response.SpreadsheetId is not null)
        {
            _spreadsheetCreationSemaphore.Release();
            return response.SpreadsheetId;
        }

        var spreadsheetId = await _sheetManagementService.CreateSpreadsheetAsync(token);
        var query = new AddGoogleTableSubjectCourseAssociation.Query(subjectCourseId, spreadsheetId);
        await _mediator.Send(query, token);

        _spreadsheetCreationSemaphore.Release();

        return spreadsheetId;
    }
}