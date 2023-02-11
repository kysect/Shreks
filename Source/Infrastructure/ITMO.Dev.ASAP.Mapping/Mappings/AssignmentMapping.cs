using ITMO.Dev.ASAP.Application.Dto.Study;
using ITMO.Dev.ASAP.Core.Study;

namespace ITMO.Dev.ASAP.Mapping.Mappings;

public static class AssignmentMapping
{
    public static AssignmentDto ToDto(this Assignment assignment)
    {
        return new AssignmentDto(
            assignment.SubjectCourse.Id,
            assignment.Id,
            assignment.Title,
            assignment.ShortName,
            assignment.Order,
            assignment.MinPoints.AsDto(),
            assignment.MaxPoints.AsDto());
    }
}