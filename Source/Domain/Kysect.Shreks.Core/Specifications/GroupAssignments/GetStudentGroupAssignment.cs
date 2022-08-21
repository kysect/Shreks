using Kysect.Shreks.Core.Study;

namespace Kysect.Shreks.Core.Specifications.GroupAssignments;

public class GetStudentGroupAssignment : ISpecification<GroupAssignment, GroupAssignment>
{
    private readonly Guid _studentId;
    private readonly Guid _assignmentId;

    public GetStudentGroupAssignment(Guid studentId, Guid assignmentId)
    {
        _studentId = studentId;
        _assignmentId = assignmentId;
    }

    public IQueryable<GroupAssignment> Apply(IQueryable<GroupAssignment> query)
    {
        return query
            .Where(x => x.Group.Students.Any(xx => xx.Id.Equals(_studentId)))
            .Where(x => x.AssignmentId.Equals(_assignmentId));
    }
}