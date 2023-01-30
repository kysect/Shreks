using Kysect.Shreks.Application.Dto.SubjectCourses;
using Kysect.Shreks.Core.Study;

namespace Kysect.Shreks.Mapping.Mappings;

public static class SubjectCourseGroupMapping
{
    public static SubjectCourseGroupDto ToDto(this SubjectCourseGroup subjectCourseGroup)
        => new SubjectCourseGroupDto(subjectCourseGroup.SubjectCourseId, subjectCourseGroup.StudentGroupId);
}