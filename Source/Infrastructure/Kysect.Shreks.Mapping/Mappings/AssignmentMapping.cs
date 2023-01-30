using Kysect.Shreks.Application.Dto.Study;
using Kysect.Shreks.Core.Study;

namespace Kysect.Shreks.Mapping.Mappings;

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
            assignment.MinPoints.Value,
            assignment.MaxPoints.Value);
    }
}