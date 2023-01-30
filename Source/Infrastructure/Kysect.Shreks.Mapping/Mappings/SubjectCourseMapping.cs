using Kysect.Shreks.Application.Dto.SubjectCourses;
using Kysect.Shreks.Core.Study;

namespace Kysect.Shreks.Mapping.Mappings;

public static class SubjectCourseMapping
{
    public static SubjectCourseDto ToDto(this SubjectCourse subjectCourse)
    {
        return new SubjectCourseDto(
            subjectCourse.Id,
            subjectCourse.Subject.Id,
            subjectCourse.Title,
            subjectCourse.WorkflowType?.AsDto(),
            subjectCourse.Associations.Select(x => x.ToDto()).ToArray());
    }
}