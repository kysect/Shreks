using Kysect.Shreks.Application.Abstractions.Google.Commands;
using Kysect.Shreks.Application.Abstractions.Google.Queries;
using Kysect.Shreks.Application.Dto.Tables;
using Kysect.Shreks.Integration.Google.Sheets;
using Kysect.Shreks.Integration.Google.Tools;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Kysect.Shreks.Integration.Google;

public class GoogleTableAccessor : IDisposable
{
    private readonly SemaphoreSlim _spreadsheetCreationSemaphore;

    private readonly ISheet<CoursePointsDto> _pointsSheet;
    private readonly ISheet<SubmissionsQueueDto> _queueSheet;
    private readonly ISheetManagementService _sheetManagementService;
    private readonly IMediator _mediator;
    private readonly ILogger<GoogleTableAccessor> _logger;

    public GoogleTableAccessor(
        ISheet<CoursePointsDto> pointsSheet,
        ISheet<SubmissionsQueueDto> queueSheet,
        ISheetManagementService sheetManagementService,
        IMediator mediator,
        ILogger<GoogleTableAccessor> logger)
    {
        _pointsSheet = pointsSheet;
        _queueSheet = queueSheet;
        _sheetManagementService = sheetManagementService;
        _mediator = mediator;
        _logger = logger;
        
        _spreadsheetCreationSemaphore = new SemaphoreSlim(1, 1);
    }

    public async Task UpdatePointsAsync(Guid subjectCourseId, CancellationToken token = default)
    {
        try
        {
            var query = new GetCoursePointsBySubjectCourse.Query(subjectCourseId);
            var response = await _mediator.Send(query, token);

            var points = response.Points;
            var spreadsheetId = await GetSpreadsheetIdAsync(subjectCourseId, token);
            await _pointsSheet.UpdateAsync(spreadsheetId, points, token);

            _logger.LogInformation("Successfully updated points sheet of course {SubjectCourseId}.", subjectCourseId);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to update points sheet of course {SubjectCourseId}.", subjectCourseId);
        }
    }

    public async Task UpdateQueueAsync(Guid subjectCourseId, CancellationToken token = default)
    {
        try
        {
            var query = new GetSubmissionsQueueBySubjectCourse.Query(subjectCourseId);
            var response = await _mediator.Send(query, token);

            var queue = response.Queue;
            var spreadsheetId = await GetSpreadsheetIdAsync(subjectCourseId, token);
            await _queueSheet.UpdateAsync(spreadsheetId, queue, token);

            _logger.LogInformation("Successfully updated queue sheet of course {SubjectCourseId}.", subjectCourseId);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to update queue sheet of course {SubjectCourseId}.", subjectCourseId);
        }
    }

    public void Dispose()
        => _spreadsheetCreationSemaphore.Dispose();

    private async Task<string> GetSpreadsheetIdAsync(Guid subjectCourseId, CancellationToken token)
    {
        var getTableInfoQuery = new GetSubjectCourseTableInfoById.Query(subjectCourseId);

        var (_, spreadsheetId) = await _mediator.Send(getTableInfoQuery, token);

        if (spreadsheetId is not null)
            return spreadsheetId;

        await _spreadsheetCreationSemaphore.WaitAsync(token);

        (var subjectCourseName, spreadsheetId) = await _mediator.Send(getTableInfoQuery, token);

        if (spreadsheetId is not null)
        {
            _spreadsheetCreationSemaphore.Release();
            return spreadsheetId;
        }

        try
        {
            spreadsheetId = await _sheetManagementService.CreateSpreadsheetAsync(subjectCourseName, token);
            var addTableCommand = new AddGoogleTableSubjectCourseAssociation.Command(subjectCourseId, spreadsheetId);
            await _mediator.Send(addTableCommand, token);

            _logger.LogInformation("Successfully created table of course {SubjectCourseId}.", subjectCourseId);
            return spreadsheetId;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to create table of course {SubjectCourseId}.", subjectCourseId);
            throw;
        }
        finally
        {
            _spreadsheetCreationSemaphore.Release();
        }
    }
}