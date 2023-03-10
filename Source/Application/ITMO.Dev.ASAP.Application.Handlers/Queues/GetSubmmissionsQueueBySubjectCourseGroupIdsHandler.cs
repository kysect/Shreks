using ITMO.Dev.ASAP.Application.Dto.Tables;
using ITMO.Dev.ASAP.Core.Queue;
using ITMO.Dev.ASAP.Core.Queue.Building;
using ITMO.Dev.ASAP.Core.Study;
using ITMO.Dev.ASAP.Core.Submissions;
using ITMO.Dev.ASAP.DataAccess.Abstractions;
using ITMO.Dev.ASAP.DataAccess.Abstractions.Extensions;
using ITMO.Dev.ASAP.Mapping.Mappings;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static ITMO.Dev.ASAP.Application.Contracts.Study.Queues.Queries.GetSubmmissionsQueueBySubjectCourseGroupIds;

namespace ITMO.Dev.ASAP.Application.Handlers.Queues;

internal class GetSubmmissionsQueueBySubjectCourseGroupIdsHandler : IRequestHandler<Query, Response>
{
    private readonly IDatabaseContext _context;
    private readonly IQueryExecutor _queryExecutor;

    public GetSubmmissionsQueueBySubjectCourseGroupIdsHandler(
        IDatabaseContext context,
        IQueryExecutor queryExecutor)
    {
        _context = context;
        _queryExecutor = queryExecutor;
    }

    public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
    {
        StudentGroup group = await _context.StudentGroups.GetByIdAsync(request.GroupId, cancellationToken);
        SubmissionQueue queue = new DefaultQueueBuilder(group, request.SubjectCourseId).Build();

        IEnumerable<Submission> submissions = await queue.UpdateSubmissions(
            _context.Submissions, _queryExecutor, cancellationToken);

        QueueSubmissionDto[] submissionsDto = submissions
            .Select(x => x.ToQueueDto())
            .ToArray();

        string groupName = await _context.StudentGroups
            .Where(x => x.Id.Equals(request.GroupId))
            .Select(x => x.Name)
            .FirstAsync(cancellationToken);

        var submissionsQueue = new SubmissionsQueueDto(groupName, submissionsDto);

        return new Response(submissionsQueue);
    }
}