using AutoMapper;
using Kysect.Shreks.Application.Dto.Study;
using Kysect.Shreks.Application.Dto.Tables;
using Kysect.Shreks.Application.Dto.Users;
using Kysect.Shreks.Core.Submissions;
using Kysect.Shreks.Core.Users;
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
            .Select(FindAssignmentPoints)
            .Where(a => a is not null)
            .Select(a => a!)
            .ToArray();

        var studentDto = _mapper.Map<StudentDto>(student);

        return new StudentPointsDto(studentDto, assignmentPoints);
    }

    private AssignmentPointsDto? FindAssignmentPoints(IEnumerable<Submission> submissions)
    {
        Submission? submission = submissions
            .Where(s => s.Points is not null)
            .MaxBy(s => s.SubmissionDate);

        //TODO: add deadlines usage instead of .Last

        if (submission is null || submission.Points is null)
            return null;

        var points = submission.Points.Value.Value;

        var submissionDate = submission.SubmissionDate;
        return new AssignmentPointsDto(submission.GroupAssignment.AssignmentId, submissionDate, points);
    }
}