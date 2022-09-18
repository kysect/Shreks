using Kysect.Shreks.Application.Abstractions.Google.Commands;
using Kysect.Shreks.Application.Abstractions.Google.Queries;
using Kysect.Shreks.Application.Dto.Tables;
using Kysect.Shreks.Application.TableManagement;
using Kysect.Shreks.Integration.Google.Sheets;
using Kysect.Shreks.Integration.Google.Tools;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Kysect.Shreks.Integration.Google;

public class GoogleTableAccessor : ITableAccessor
{
    private readonly SemaphoreSlim _spreadsheetCreationSemaphore;

    private readonly ISheet<CoursePointsDto> _pointsSheet;
    private readonly ISheet<SubmissionsQueueDto> _queueSheet;
    private readonly ISpreadsheetManagementService _spreadsheetManagementService;
    private readonly IMediator _mediator;
    private readonly ILogger<GoogleTableAccessor> _logger;

    public GoogleTableAccessor(
        ISheet<CoursePointsDto> pointsSheet,
        ISheet<SubmissionsQueueDto> queueSheet,
        ISpreadsheetManagementService spreadsheetManagementService,
        IMediator mediator,
        ILogger<GoogleTableAccessor> logger)
    {
        _pointsSheet = pointsSheet;
        _queueSheet = queueSheet;
        _spreadsheetManagementService = spreadsheetManagementService;
        _mediator = mediator;
        _logger = logger;
        
        _spreadsheetCreationSemaphore = new SemaphoreSlim(1, 1);
    }

    public async Task UpdatePointsAsync(Guid subjectCourseId, CancellationToken token)
    {
        try
        {
            _logger.LogInformation("Start updating for points sheet of course {SubjectCourseId}.", subjectCourseId);

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

    public async Task UpdateQueueAsync(Guid subjectCourseId, Guid studentGroupId, CancellationToken token)
    {
        try
        {
            _logger.LogInformation("Start updating for queue sheet of group: {GroupId}, course: {CourseId}.", studentGroupId, subjectCourseId);

            var query = new GetSubjectCourseGroupSubmissionQueue.Query(subjectCourseId, studentGroupId);
            var response = await _mediator.Send(query, token);

            var queue = response.Queue;
            var spreadsheetId = await GetSpreadsheetIdAsync(subjectCourseId, token);
            await _queueSheet.UpdateAsync(spreadsheetId, queue, token);

            const string infoMessage = "Successfully updated queue sheet of group: {GroupId}, course: {CourseId}.";
            _logger.LogInformation(infoMessage, studentGroupId, subjectCourseId);
        }
        catch (Exception e)
        {
            const string errorMessage = "Failed to update queue sheet of group: {GroupId}, course: {CourseId}.";
            _logger.LogError(e, errorMessage, studentGroupId, subjectCourseId);
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

        _logger.LogInformation("Spreadsheet of course {SubjectCourseId} was not found and will be created.", subjectCourseId);

        try
        {
            spreadsheetId = await _spreadsheetManagementService.CreateSpreadsheetAsync(subjectCourseName, token);
            var addTableCommand = new AddGoogleTableSubjectCourseAssociation.Command(subjectCourseId, spreadsheetId);
            await _mediator.Send(addTableCommand, token);

            _logger.LogInformation("Successfully created spreadsheet of course {SubjectCourseId}.", subjectCourseId);
            return spreadsheetId;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to create spreadsheet of course {SubjectCourseId}.", subjectCourseId);
            throw;
        }
        finally
        {
            _spreadsheetCreationSemaphore.Release();
        }
    }
}