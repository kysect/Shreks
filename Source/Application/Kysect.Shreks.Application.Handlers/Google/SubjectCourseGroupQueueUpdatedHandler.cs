using AutoMapper;
using Kysect.Shreks.Application.Abstractions.Google;
using Kysect.Shreks.Application.Abstractions.Google.Notifications;
using Kysect.Shreks.Application.Abstractions.Google.Sheets;
using Kysect.Shreks.Application.Dto.Tables;
using Kysect.Shreks.Core.Queue;
using Kysect.Shreks.Core.Queue.Building;
using Kysect.Shreks.Core.Study;
using Kysect.Shreks.Core.Submissions;
using Kysect.Shreks.DataAccess.Abstractions;
using Kysect.Shreks.DataAccess.Abstractions.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Kysect.Shreks.Application.Handlers.Google;

internal class SubjectCourseGroupQueueUpdatedHandler : INotificationHandler<SubjectCourseGroupQueueUpdatedNotification>
{
    private readonly IShreksDatabaseContext _context;
    private readonly ILogger<SubjectCourseGroupQueueUpdatedHandler> _logger;
    private readonly IMapper _mapper;
    private readonly IQueryExecutor _queryExecutor;
    private readonly ISheet<SubmissionsQueueDto> _sheet;
    private readonly ISubjectCourseTableService _subjectCourseTableService;

    public SubjectCourseGroupQueueUpdatedHandler(
        IShreksDatabaseContext context,
        IQueryExecutor queryExecutor,
        IMapper mapper,
        ISheet<SubmissionsQueueDto> sheet,
        ISubjectCourseTableService subjectCourseTableService,
        ILogger<SubjectCourseGroupQueueUpdatedHandler> logger)
    {
        _context = context;
        _queryExecutor = queryExecutor;
        _mapper = mapper;
        _sheet = sheet;
        _subjectCourseTableService = subjectCourseTableService;
        _logger = logger;
    }

    public async Task Handle(
        SubjectCourseGroupQueueUpdatedNotification notification,
        CancellationToken cancellationToken)
    {
        try
        {
            await ExecuteAsync(notification, cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogError(
                e,
                "Error while updating queue for subject course {SubjectCourseId} group {GroupId}",
                notification.SubjectCourseId,
                notification.GroupId);
        }
    }

    private async Task ExecuteAsync(
        SubjectCourseGroupQueueUpdatedNotification notification,
        CancellationToken cancellationToken)
    {
        StudentGroup group = await _context.StudentGroups.GetByIdAsync(notification.GroupId, cancellationToken);
        SubmissionQueue queue = new DefaultQueueBuilder(group, notification.SubjectCourseId).Build();

        IEnumerable<Submission> submissions = await queue.UpdateSubmissions(
            _context.Submissions, _queryExecutor, cancellationToken);

        QueueSubmissionDto[] submissionsDto = submissions
            .Select(_mapper.Map<QueueSubmissionDto>)
            .ToArray();

        string groupName = await _context.StudentGroups
            .Where(x => x.Id.Equals(notification.GroupId))
            .Select(x => x.Name)
            .FirstAsync(cancellationToken);

        var submissionsQueue = new SubmissionsQueueDto(groupName, submissionsDto);

        string spreadsheetId = await _subjectCourseTableService
            .GetSubjectCourseTableId(notification.SubjectCourseId, cancellationToken);

        await _sheet.UpdateAsync(spreadsheetId, submissionsQueue, cancellationToken);
    }
}