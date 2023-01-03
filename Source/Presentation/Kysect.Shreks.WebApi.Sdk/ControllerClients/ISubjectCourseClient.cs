using Kysect.Shreks.Application.Dto.Study;
using Kysect.Shreks.Application.Dto.SubjectCourses;
using Kysect.Shreks.Application.Dto.Users;
using Kysect.Shreks.WebApi.Abstractions.Models.SubjectCourses;

namespace Kysect.Shreks.WebApi.Sdk.ControllerClients;

public interface ISubjectCourseClient
{
    Task<SubjectCourseDto> CreateAsync(
        CreateSubjectCourseRequest request,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<SubjectCourseDto>> GetAsync(CancellationToken cancellationToken = default);

    Task<SubjectCourseDto> GetAsync(Guid id, CancellationToken cancellationToken = default);

    Task<SubjectCourseDto> UpdateAsync(
        Guid id,
        UpdateSubjectCourseRequest request,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<StudentDto>> GetStudentsAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<AssignmentDto>>
        GetAssignmentsAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<SubjectCourseGroupDto>> GetGroupsAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<SubjectCourseDto> AddGithubAssociationAsync(
        Guid id,
        AddSubjectCourseGithubAssociationRequest request,
        CancellationToken cancellationToken = default);

    Task<SubjectCourseDto> RemoveGithubAssociationAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task AddFractionDeadlinePolicyAsync(
        Guid id,
        AddFractionPolicyRequest request,
        CancellationToken cancellationToken = default);
}