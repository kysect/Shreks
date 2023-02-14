using ITMO.Dev.ASAP.Application.Dto.Study;
using ITMO.Dev.ASAP.Application.Dto.SubjectCourses;
using ITMO.Dev.ASAP.WebApi.Abstractions.Models.Subjects;

namespace ITMO.Dev.ASAP.WebApi.Sdk.ControllerClients;

public interface ISubjectClient
{
    Task<SubjectDto> CreateAsync(CreateSubjectRequest request, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<SubjectDto>> GetAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<SubjectDto>> GetAsync(Guid id, CancellationToken cancellationToken = default);

    Task<SubjectDto> UpdateAsync(UpdateSubjectRequest request, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<SubjectCourseDto>> GetCoursesAsync(
        Guid subjectId,
        CancellationToken cancellationToken = default);
}