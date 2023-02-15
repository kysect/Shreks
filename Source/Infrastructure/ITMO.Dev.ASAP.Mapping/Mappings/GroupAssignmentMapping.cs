using ITMO.Dev.ASAP.Application.Dto.Study;
using ITMO.Dev.ASAP.Core.Study;

namespace ITMO.Dev.ASAP.Mapping.Mappings;

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