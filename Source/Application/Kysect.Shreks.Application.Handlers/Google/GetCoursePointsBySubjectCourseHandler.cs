using AutoMapper;
using Kysect.Shreks.Application.Dto.Study;
using Kysect.Shreks.Application.Dto.Tables;
using Kysect.Shreks.Application.Dto.Users;
using Kysect.Shreks.Core.DeadlinePolicies;
using Kysect.Shreks.Core.Study;
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

        var submission = _context.Submissions
            .Where(s => assignments.Any(a => a.Equals(s.Assignment)));

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
            .GroupBy(s => s.Assignment)
            .Select(g => GetAssignmentPoints(g.Key, student, g))
            .ToArray();

        var studentDto = _mapper.Map<StudentDto>(student);

        return new StudentPointsDto(studentDto, assignmentPoints);
    }

    private AssignmentPointsDto GetAssignmentPoints(Assignment assignment, Student student, IEnumerable<Submission> submissions)
    {
        var group = student.Group;

        var deadline = assignment.GroupAssignments.First(g => g.Group.Equals(group)).Deadline;

        var (submission, points) = submissions
            .Select(s => (submission: s, points: GetSubmissionPoints(s, deadline)))
            .MaxBy(s => s.points);

        return new AssignmentPointsDto(assignment.Id, submission.SubmissionDate, points.Value);
    }

    private Points GetSubmissionPoints(Submission submission, DateOnly deadline)
    {
        var deadlinePolicy = GetActiveDeadlinePolicy(submission, deadline);

        if (deadlinePolicy is null)
            return submission.Points;

        var points = deadlinePolicy.Apply(submission.Points);

        return points;
    }

    private DeadlinePolicy? GetActiveDeadlinePolicy(Submission submission, DateOnly deadline)
    {
        if (submission.SubmissionDate <= deadline)
            return null;

        var submissionDeadlineOffset = TimeSpan.FromDays(submission.SubmissionDate.DayNumber - deadline.DayNumber);
        return submission
            .Assignment
            .DeadlinePolicies
            .Where(dp => dp.SpanBeforeActivation < submissionDeadlineOffset)
            .MaxBy(dp => dp.SpanBeforeActivation);
    }
}