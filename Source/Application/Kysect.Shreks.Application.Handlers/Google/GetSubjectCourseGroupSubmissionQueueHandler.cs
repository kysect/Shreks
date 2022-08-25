using AutoMapper;
using Kysect.Shreks.Application.Dto.Study;
using Kysect.Shreks.Application.Dto.Tables;
using Kysect.Shreks.Application.Dto.Users;
using Kysect.Shreks.DataAccess.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static Kysect.Shreks.Application.Abstractions.Google.Queries.GetSubjectCourseGroupSubmissionQueue;

namespace Kysect.Shreks.Application.Handlers.Google;

public class GetSubjectCourseGroupSubmissionQueueHandler : IRequestHandler<Query, Response>
{
    private readonly IShreksDatabaseContext _context;
    private readonly IMapper _mapper;

    public GetSubjectCourseGroupSubmissionQueueHandler(IShreksDatabaseContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
    {
        var (courseId, groupId) = request;

        var groupName = _context.StudentGroups.First(g => g.Id == groupId).Name;
        
        var submissions = await _context.Submissions
            .Where(s => s.Student.Group.Id == groupId
                        && s.GroupAssignment.Assignment.SubjectCourse.Id == courseId)
            .ToArrayAsync(cancellationToken);
        
        //TODO: add queue logic

        var queueSubmissionsDto = submissions
            .Select(s => (
                Submission: _mapper.Map<SubmissionDto>(s),
                Student: _mapper.Map<StudentDto>(s.Student)))
            .Select(t => new QueueSubmissionDto(t.Student, t.Submission))
            .ToArray();
        
        var submissionsQueue = new SubmissionsQueueDto(groupName, queueSubmissionsDto);
        return new Response(submissionsQueue);
    }
}