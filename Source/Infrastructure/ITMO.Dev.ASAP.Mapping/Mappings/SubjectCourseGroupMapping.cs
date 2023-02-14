using ITMO.Dev.ASAP.Application.Dto.SubjectCourses;
using ITMO.Dev.ASAP.Core.Study;

namespace ITMO.Dev.ASAP.Mapping.Mappings;

public static class SubjectCourseGroupMapping
{
    public static SubjectCourseGroupDto ToDto(this SubjectCourseGroup subjectCourseGroup)
    {
        return new SubjectCourseGroupDto(subjectCourseGroup.SubjectCourseId, subjectCourseGroup.StudentGroupId);
    }
}