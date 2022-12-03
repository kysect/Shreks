using Kysect.Shreks.Application.Dto.SubjectCourses;

namespace Kysect.Shreks.Application.Abstractions.SubjectCourses;

public interface ISubjectCourseService
{
    Task<SubjectCoursePointsDto> CalculatePointsAsync(Guid subjectCourseId, CancellationToken cancellationToken);
}