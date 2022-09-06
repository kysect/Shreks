using AutoMapper;
using Kysect.Shreks.Application.Dto.Study;
using Kysect.Shreks.Application.Dto.Tables;
using Kysect.Shreks.Application.Dto.Users;
using Kysect.Shreks.Application.Extensions;
using Kysect.Shreks.Core.Models;
using Kysect.Shreks.Core.Study;
using Kysect.Shreks.Core.Submissions;
using Kysect.Shreks.Core.Users;
using Kysect.Shreks.Core.ValueObject;
using Kysect.Shreks.DataAccess.Abstractions;
using Kysect.Shreks.DataAccess.Abstractions.Extensions;
using MediatR;
using Microsoft.Extensions.Logging;
using static Kysect.Shreks.Application.Abstractions.Google.Queries.GetCoursePointsBySubjectCourse;

namespace Kysect.Shreks.Application.Handlers.Google;

public class GetCoursePointsBySubjectCourseHandler : IRequestHandler<Query, Response>
{
    private readonly IShreksDatabaseContext _context;
    private readonly IMapper _mapper;
    private readonly ILogger<GetCoursePointsBySubjectCourseHandler> _logger;

    public GetCoursePointsBySubjectCourseHandler(IShreksDatabaseContext context, IMapper mapper, ILogger<GetCoursePointsBySubjectCourseHandler> logger)
    {
        _context = context;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        _logger.LogInformation("Started to collecting all course {courseId} points", request.SubjectCourseId.ToString("D"));

        var subjectCourse = await _context.SubjectCourses.GetByIdAsync(request.SubjectCourseId, cancellationToken);

        var assignments = subjectCourse.Assignments;

        List<Submission> submission = assignments
            .SelectMany(a => a.GroupAssignments)
            .SelectMany(ga => ga.Submissions)
            .Where(s => s.State == SubmissionState.Completed)
            .ToList();

        var studentPoints = subjectCourse.Groups
            .SelectMany(g => g.StudentGroup.Students)
            .Select(s => GetStudentPoints(s, submission))
            .ToArray();

        var assignmentsDto = subjectCourse.Assignments
            .Select(_mapper.Map<AssignmentDto>)
            .ToArray();

        _logger.LogInformation("Finished to collect all course {courseId} points", request.SubjectCourseId.ToString("D"));

        var points = new CoursePointsDto(assignmentsDto, studentPoints);
        return new Response(points);
    }

    private StudentPointsDto GetStudentPoints(Student student, IReadOnlyCollection<Submission> submissions)
    {
        var assignmentPoints = submissions
            .Where(s => s.Student.Equals(student))
            .GroupBy(s => s.GroupAssignment)
            .Select(g => FindAssignmentPoints(g.Key, g))
            .Where(a => a is not null)
            .Select(a => a!)
            .ToArray();

        var studentDto = _mapper.Map<StudentDto>(student);

        return new StudentPointsDto(studentDto, assignmentPoints);
    }

    private AssignmentPointsDto? FindAssignmentPoints(GroupAssignment groupAssignment, IEnumerable<Submission> submissions)
    {
        var deadline = groupAssignment.Deadline;

        (var submission, Points? points) = submissions
            .Select(s => (submission: s, points: s.CalculateTotalSubmissionPoints(deadline)))
            .OrderByDescending(x => x.points)
            .FirstOrDefault();

        if (points is null)
            return null;

        return new AssignmentPointsDto(groupAssignment.AssignmentId, submission.SubmissionDateOnly, points.Value.Value);
    }
}