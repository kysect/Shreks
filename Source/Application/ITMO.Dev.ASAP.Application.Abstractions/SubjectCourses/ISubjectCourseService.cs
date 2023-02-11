using ITMO.Dev.ASAP.Application.Dto.SubjectCourses;

namespace ITMO.Dev.ASAP.Application.Abstractions.SubjectCourses;

public interface ISubjectCourseService
{
    Task<SubjectCoursePointsDto> CalculatePointsAsync(Guid subjectCourseId, CancellationToken cancellationToken);
}