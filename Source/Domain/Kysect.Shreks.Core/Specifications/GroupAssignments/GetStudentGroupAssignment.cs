using Kysect.Shreks.Core.Study;

namespace Kysect.Shreks.Core.Specifications.GroupAssignments;

public class GetStudentGroupAssignment : ISpecification<GroupAssignment, GroupAssignment>
{
    private readonly Guid _userId;
    private readonly Guid _assignmentId;

    public GetStudentGroupAssignment(Guid userId, Guid assignmentId)
    {
        _userId = userId;
        _assignmentId = assignmentId;
    }

    public IQueryable<GroupAssignment> Apply(IQueryable<GroupAssignment> query)
    {
        return query
            .Where(x => x.Group.Students.Any(xx => xx.UserId.Equals(_userId)))
            .Where(x => x.AssignmentId.Equals(_assignmentId));
    }
}