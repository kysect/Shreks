using AutoMapper;
using Kysect.Shreks.Application.Abstractions.Exceptions;
using Kysect.Shreks.Application.Dto.Study;
using Kysect.Shreks.Application.Dto.Tables;
using Kysect.Shreks.Application.Dto.Users;
using Kysect.Shreks.Core.Queue;
using Kysect.Shreks.DataAccess.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static Kysect.Shreks.Application.Abstractions.Google.Queries.GetSubjectCourseGroupSubmissionQueue;

namespace Kysect.Shreks.Application.Handlers.Google;

public class GetSubjectCourseGroupSubmissionQueueHandler : IRequestHandler<Query, Response>
{
    private readonly IShreksDatabaseContext _context;
    private readonly IMapper _mapper;
    private readonly IQueryExecutor _queryExecutor;

    public GetSubjectCourseGroupSubmissionQueueHandler(IShreksDatabaseContext context, IMapper mapper, IQueryExecutor queryExecutor)
    {
        _context = context;
        _mapper = mapper;
        _queryExecutor = queryExecutor;
    }

    public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
    {
        var queue = await _context.SubjectCourseGroups
            .Where(x => x.SubjectCourseId.Equals(request.SubjectCourseId))
            .Where(x => x.StudentGroupId.Equals(request.StudentGroupId))
            .Select(x => x.Queue)
            .FirstOrDefaultAsync(cancellationToken);

        if (queue is null)
            throw new EntityNotFoundException("Queue for specified subject course group was not found");

        await queue.UpdateSubmissions(_context.Submissions, _queryExecutor, cancellationToken);

        var submissions = queue.Submissions
            .OrderBy(x => x.Position)
            .Select(x => x.Submission);

        var queueSubmissionsDto = submissions
            .Select(s => (
                Submission: _mapper.Map<SubmissionDto>(s),
                Student: _mapper.Map<StudentDto>(s.Student)))
            .Select(t => new QueueSubmissionDto(t.Student, t.Submission))
            .ToArray();

        var groupName = await _context.StudentGroups
            .Where(x => x.Id.Equals(request.StudentGroupId))
            .Select(x => x.Name)
            .FirstAsync(cancellationToken);

        var submissionsQueue = new SubmissionsQueueDto(groupName, queueSubmissionsDto);
        return new Response(submissionsQueue);
    }
}