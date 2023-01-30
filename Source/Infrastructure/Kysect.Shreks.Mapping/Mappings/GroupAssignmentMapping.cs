using Kysect.Shreks.Application.Dto.Study;
using Kysect.Shreks.Core.Study;

namespace Kysect.Shreks.Mapping.Mappings;

public static class GroupAssignmentMapping
{
    public static GroupAssignmentDto ToDto(this GroupAssignment groupAssignment)
    {
        return new GroupAssignmentDto(
            groupAssignment.GroupId,
            groupAssignment.Group.Name,
            groupAssignment.AssignmentId,
            groupAssignment.Assignment.Title,
            groupAssignment.Deadline.AsDateTime());
    }
}