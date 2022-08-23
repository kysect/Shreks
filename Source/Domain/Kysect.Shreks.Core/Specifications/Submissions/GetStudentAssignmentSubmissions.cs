using Kysect.Shreks.Core.Submissions;

namespace Kysect.Shreks.Core.Specifications.Submissions;

public class GetStudentAssignmentSubmissions : ISpecification<Submission, Submission>
{
    private readonly Guid _studentId;
    private readonly Guid _assignmentId;

    public GetStudentAssignmentSubmissions(Guid studentId, Guid assignmentId)
    {
        _studentId = studentId;
        _assignmentId = assignmentId;
    }

    public IQueryable<Submission> Apply(IQueryable<Submission> query)
    {
        return query
            .Where(x => x.Student.Id.Equals(_studentId))
            .Where(x => x.GroupAssignment.AssignmentId.Equals(_assignmentId));
    }
}