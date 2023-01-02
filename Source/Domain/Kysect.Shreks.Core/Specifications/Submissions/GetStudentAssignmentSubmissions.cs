using Kysect.Shreks.Core.Submissions;

namespace Kysect.Shreks.Core.Specifications.Submissions;

public class GetStudentAssignmentSubmissions : ISpecification<Submission, Submission>
{
    private readonly Guid _assignmentId;
    private readonly Guid _userId;

    public GetStudentAssignmentSubmissions(Guid userId, Guid assignmentId)
    {
        _userId = userId;
        _assignmentId = assignmentId;
    }

    public IQueryable<Submission> Apply(IQueryable<Submission> query)
    {
        return query
            .Where(x => x.Student.UserId.Equals(_userId))
            .Where(x => x.GroupAssignment.AssignmentId.Equals(_assignmentId));
    }
}