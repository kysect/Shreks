using Kysect.Shreks.Application.Dto.Study;
using Kysect.Shreks.Core.Study;

namespace Kysect.Shreks.Mapping.Mappings;

public static class StudyGroupMapping
{
    public static StudyGroupDto ToDto(this StudentGroup group)
        => new StudyGroupDto(group.Id, group.Name);
}