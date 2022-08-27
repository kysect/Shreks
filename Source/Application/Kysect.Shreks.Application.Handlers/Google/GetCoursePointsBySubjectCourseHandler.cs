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
using static Kysect.Shreks.Application.Abstractions.Google.Queries.GetCoursePointsBySubjectCourse;

namespace Kysect.Shreks.Application.Handlers.Google;

public class GetCoursePointsBySubjectCourseHandler : IRequestHandler<Query, Response>
{
    private readonly IShreksDatabaseContext _context;
    private readonly IMapper _mapper;

    public GetCoursePointsBySubjectCourseHandler(IShreksDatabaseContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var subjectCourse = await _context.SubjectCourses.GetByIdAsync(request.SubjectCourseId, cancellationToken);

        var assignments = subjectCourse.Assignments;

        // TODO: It is not evident query. We can fetch submissions from subject course.
        var submission = _context.Submissions
            .Where(s => s.State == SubmissionState.Completed)
            .Where(s => assignments.Any(a => a.Equals(s.GroupAssignment.Assignment)));

        var studentPoints = subjectCourse.Groups
            .SelectMany(g => g.StudentGroup.Students.Select(s => GetStudentPoints(s, submission)))
            .ToArray();

        var assignmentsDto = subjectCourse.Assignments
            .Select(_mapper.Map<AssignmentDto>)
            .ToArray();

        var points = new CoursePointsDto(assignmentsDto, studentPoints);
        return new Response(points);
    }

    private StudentPointsDto GetStudentPoints(Student student, IQueryable<Submission> submissions)
    {
        var assignmentPoints = submissions
            .Where(s => s.Student.Equals(student))
            .AsEnumerable()
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

        return new AssignmentPointsDto(groupAssignment.AssignmentId, submission.SubmissionDate, points.Value.Value);
    }
}