using Kysect.Shreks.Application.Abstractions.Google;
using Kysect.Shreks.Application.Abstractions.Google.Notifications;
using Kysect.Shreks.Application.Abstractions.Google.Sheets;
using Kysect.Shreks.Application.Abstractions.SubjectCourses;
using Kysect.Shreks.Application.Dto.Study;
using Kysect.Shreks.Application.Dto.SubjectCourses;
using Kysect.Shreks.Application.Dto.Tables;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Kysect.Shreks.Application.Handlers.Google;

internal class SubjectCoursePointsUpdatedHandler : INotificationHandler<SubjectCoursePointsUpdatedNotification>
{
    private readonly ILogger<SubjectCoursePointsUpdatedHandler> _logger;
    private readonly ISubjectCourseService _service;
    private readonly ISheet<SubjectCoursePointsDto> _sheet;
    private readonly ISubjectCourseTableService _subjectCourseTableService;

    public SubjectCoursePointsUpdatedHandler(
        ISubjectCourseTableService subjectCourseTableService,
        ILogger<SubjectCoursePointsUpdatedHandler> logger,
        ISheet<SubjectCoursePointsDto> sheet,
        ISubjectCourseService service)
    {
        _subjectCourseTableService = subjectCourseTableService;
        _logger = logger;
        _sheet = sheet;
        _service = service;
    }

    public async Task Handle(SubjectCoursePointsUpdatedNotification notification, CancellationToken cancellationToken)
    {
        try
        {
            await ExecuteAsync(notification, cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error updating course points for subject course {SubjectCourseId}",
                notification.SubjectCourseId);
        }
    }

    private async Task ExecuteAsync(
        SubjectCoursePointsUpdatedNotification notification,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Start updating for points sheet of course {SubjectCourseId}.",
            notification.SubjectCourseId);

        _logger.LogInformation("Started to collecting all course {courseId} points", notification.SubjectCourseId);

        SubjectCoursePointsDto points = await _service.CalculatePointsAsync(
            notification.SubjectCourseId, cancellationToken);

        _logger.LogInformation("Finished to collect all course {courseId} points", notification.SubjectCourseId);

        if (_logger.IsEnabled(LogLevel.Trace))
        {
            _logger.LogTrace("Calculated points:");

            IEnumerable<(StudentPointsDto Student, AssignmentPointsDto Points, AssignmentDto Assignment)> table =
                points.StudentsPoints.SelectMany(x => x.Points, (s, a) =>
                {
                    AssignmentDto assignment = points.Assignments.Single(x => x.Id.Equals(a.AssignmentId));
                    return (Student: s, Points: a, Assignment: assignment);
                });

            foreach ((StudentPointsDto student, AssignmentPointsDto studentPoints, AssignmentDto assignment) in table)
            {
                _logger.LogTrace("\t{Student} - {Assignment}: {Points}, banned: {Banned}",
                    student.Student.GitHubUsername, assignment.Title, studentPoints.Points, studentPoints.IsBanned);
            }
        }

        string spreadsheetId = await _subjectCourseTableService.GetSubjectCourseTableId(
            notification.SubjectCourseId, cancellationToken);

        await _sheet.UpdateAsync(spreadsheetId, points, cancellationToken);

        _logger.LogInformation("Successfully updated points sheet of course {SubjectCourseId}.",
            notification.SubjectCourseId);
    }
}