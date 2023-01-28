using Kysect.Shreks.Application.Dto.SubjectCourses;
using Kysect.Shreks.WebApi.Abstractions.Models.SubjectCourseGroups;

namespace Kysect.Shreks.WebApi.Sdk.ControllerClients;

public interface ISubjectCourseGroupClient
{
    Task<SubjectCourseGroupDto> CreateAsync(
        CreateSubjectCourseGroupRequest request,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<SubjectCourseGroupDto>> BulkCreateAsync(
        BulkCreateSubjectCourseGroupsRequest request,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(DeleteSubjectCourseGroupRequest request, CancellationToken cancellationToken = default);
}