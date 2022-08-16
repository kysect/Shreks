using Kysect.Shreks.Application.Abstractions.DataAccess;
using Kysect.Shreks.Application.Abstractions.Google;
using Kysect.Shreks.Core.Study;
using Kysect.Shreks.Core.Users;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static Kysect.Shreks.Application.Abstractions.Google.Queries.GetCoursePointsBySubjectCourse;

namespace Kysect.Shreks.Application.Handlers.Google;

public class GetCoursePointsBySubjectCourseHandler : IRequestHandler<Query, Response>
{
    private readonly IShreksDatabaseContext _context;

    public GetCoursePointsBySubjectCourseHandler(IShreksDatabaseContext context)
    {
        _context = context;
    }

    public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
    {
        var subjectCourse = await _context.SubjectCourses.GetByIdAsync(request.SubjectCourseId, cancellationToken);

        var submission = await _context.Submissions.ToArrayAsync(cancellationToken);

        var studentPoints = subjectCourse.Groups
            .SelectMany(g => g.StudentGroup.Students.Select(s => GetStudentPoints(s, submission)))
            .ToArray();

        var points = new CoursePoints(subjectCourse.Assignments, studentPoints);
        return new Response(points);
    }

    private static StudentPoints GetStudentPoints(Student student, IEnumerable<Submission> submissions)
    {
        var assignmentPoints = submissions
            .Where(s => s.Student.Equals(student))
            .GroupBy(s => s.Assignment)
            .Select(s => s.MaxBy(sub => sub.SubmissionDateTime)!)
            .Select(GetAssignmentPoints)
            .ToArray();

        return new StudentPoints(student, assignmentPoints);
    }

    private static AssignmentPoints GetAssignmentPoints(Submission submission)
    {
        var points = submission.Points;
        //TODO: add deadlines usage

        var submissionDate = DateOnly.FromDateTime(submission.SubmissionDateTime);
        return new AssignmentPoints(submission.Assignment, submissionDate, points);
    }
}