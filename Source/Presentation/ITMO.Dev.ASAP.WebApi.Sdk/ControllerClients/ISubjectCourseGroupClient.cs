using ITMO.Dev.ASAP.Application.Dto.SubjectCourses;
using ITMO.Dev.ASAP.WebApi.Abstractions.Models.SubjectCourseGroups;

namespace ITMO.Dev.ASAP.WebApi.Sdk.ControllerClients;

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