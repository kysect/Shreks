using Kysect.Shreks.Application.Dto.Study;
using Kysect.Shreks.Application.Dto.SubjectCourses;
using Kysect.Shreks.WebApi.Abstractions.Models.Subjects;

namespace Kysect.Shreks.WebApi.Sdk.ControllerClients;

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