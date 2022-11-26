using AutoMapper;
using Kysect.Shreks.Application.Abstractions.Google;
using Kysect.Shreks.Application.Abstractions.Google.Notifications;
using Kysect.Shreks.Application.Abstractions.Google.Sheets;
using Kysect.Shreks.Application.Dto.Study;
using Kysect.Shreks.Application.Dto.SubjectCourses;
using Kysect.Shreks.Application.Dto.Tables;
using Kysect.Shreks.Application.Dto.Users;
using Kysect.Shreks.Application.Extensions;
using Kysect.Shreks.Core.Study;
using Kysect.Shreks.Core.Users;
using Kysect.Shreks.DataAccess.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Kysect.Shreks.Application.Handlers.Google;

public class SubjectCoursePointsUpdatedHandler : INotificationHandler<SubjectCoursePointsUpdatedNotification>
{
    private readonly ISubjectCourseTableService _subjectCourseTableService;
    private readonly ILogger<SubjectCoursePointsUpdatedHandler> _logger;
    private readonly IShreksDatabaseContext _context;
    private readonly IMapper _mapper;
    private readonly ISheet<SubjectCoursePointsDto> _sheet;

    public SubjectCoursePointsUpdatedHandler(
        ISubjectCourseTableService subjectCourseTableService,
        ILogger<SubjectCoursePointsUpdatedHandler> logger,
        IShreksDatabaseContext context,
        IMapper mapper,
        ISheet<SubjectCoursePointsDto> sheet)
    {
        _subjectCourseTableService = subjectCourseTableService;
        _logger = logger;
        _context = context;
        _mapper = mapper;
        _sheet = sheet;
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

        List<Assignment> assignments = await _context.Assignments
            .Include(x => x.GroupAssignments)
            .ThenInclude(x => x.Group)
            .ThenInclude(x => x.Students)
            .Include(x => x.GroupAssignments)
            .ThenInclude(x => x.Submissions)
            .Where(x => x.SubjectCourse.Id.Equals(notification.SubjectCourseId))
            .ToListAsync(cancellationToken);

        IEnumerable<StudentAssignment> studentAssignmentPoints = assignments
            .SelectMany(x => x.GroupAssignments)
            .SelectMany(ga => ga.Group.Students.Select(s => new StudentAssignment(s, ga)));

        StudentPointsDto[] studentPoints = studentAssignmentPoints
            .GroupBy(x => x.Student)
            .Select(MapToStudentPoints)
            .ToArray();

        _logger.LogInformation("Finished to collect all course {courseId} points", notification.SubjectCourseId);

        AssignmentDto[] assignmentsDto = assignments.Select(_mapper.Map<AssignmentDto>).ToArray();
        var points = new SubjectCoursePointsDto(assignmentsDto, studentPoints);

        string spreadsheetId = await _subjectCourseTableService
            .GetSubjectCourseTableId(notification.SubjectCourseId, cancellationToken);

        await _sheet.UpdateAsync(spreadsheetId, points, cancellationToken);

        _logger.LogInformation("Successfully updated points sheet of course {SubjectCourseId}.",
            notification.SubjectCourseId);
    }

    private StudentPointsDto MapToStudentPoints(IGrouping<Student, StudentAssignment> grouping)
    {
        StudentDto studentDto = _mapper.Map<StudentDto>(grouping.Key);

        AssignmentPointsDto[] pointsDto = grouping
            .Select(x => x.Points)
            .WhereNotNull()
            .Select(x => new AssignmentPointsDto(x.Assignment.Id, x.SubmissionDate, x.IsBanned, x.Points.Value))
            .ToArray();

        return new StudentPointsDto(studentDto, pointsDto);
    }
}