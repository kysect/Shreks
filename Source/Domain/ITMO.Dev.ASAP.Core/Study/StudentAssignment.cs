using ITMO.Dev.ASAP.Common.Exceptions;
using ITMO.Dev.ASAP.Core.Models;
using ITMO.Dev.ASAP.Core.Submissions;
using ITMO.Dev.ASAP.Core.Users;
using ITMO.Dev.ASAP.Core.ValueObject;
using RichEntity.Annotations;

namespace ITMO.Dev.ASAP.Core.Study;

public partial class StudentAssignment : IEntity
{
    public StudentAssignment(Student student, GroupAssignment assignment)
        : this(assignment.GroupId, assignment.AssignmentId, student.UserId)
    {
        if (assignment.Group.Students.Contains(student) is false)
        {
            string studentString = student.ToString();
            string groupString = assignment.Group.ToString();
            throw StudentAssignmentException.StudentGroupAssignmentMismatch(studentString, groupString);
        }

        Student = student;
        Assignment = assignment;
    }

    [KeyProperty]
    public Student Student { get; }

    [KeyProperty]
    public GroupAssignment Assignment { get; }

    public StudentAssignmentPoints? Points => CalculatePoints();

    private StudentAssignmentPoints? CalculatePoints()
    {
        IEnumerable<Submission> submissions = Assignment.Submissions
            .Where(x => x.Student.Equals(Student))
            .Where(x => x.State.IsTerminalEffectiveState);

        (Submission submission, Points? points, bool isBanned) = submissions
            .Select(s => (submission: s, points: s.EffectivePoints, isBanned: s.State.Kind is SubmissionStateKind.Banned))
            .OrderByDescending(x => x.isBanned)
            .ThenByDescending(x => x.points)
            .FirstOrDefault();

        if (points is null && isBanned is false)
            return null;

        if (isBanned)
            points = null;

        return new StudentAssignmentPoints(
            Student,
            Assignment.Assignment,
            isBanned,
            points ?? ValueObject.Points.None,
            submission.SubmissionDateOnly);
    }
}