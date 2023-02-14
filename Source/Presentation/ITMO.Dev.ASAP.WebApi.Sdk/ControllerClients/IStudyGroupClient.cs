using ITMO.Dev.ASAP.Application.Dto.Querying;
using ITMO.Dev.ASAP.Application.Dto.Study;
using ITMO.Dev.ASAP.Application.Dto.Users;
using ITMO.Dev.ASAP.WebApi.Abstractions.Models.StudyGroups;

namespace ITMO.Dev.ASAP.WebApi.Sdk.ControllerClients;

public interface IStudyGroupClient
{
    Task<StudyGroupDto> CreateAsync(CreateStudyGroupRequest request, CancellationToken cancellationToken = default);

    Task<StudyGroupDto> GetAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<StudyGroupDto>> GetAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<StudyGroupDto>> GetAsync(
        IEnumerable<Guid> ids,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<StudentDto>> GetStudentsAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<GroupAssignmentDto>> GetAssignmentsAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<StudyGroupDto?> FindAsync(string name, CancellationToken cancellationToken = default);

    Task<StudyGroupDto> UpdateAsync(
        Guid id,
        UpdateStudyGroupRequest request,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<StudyGroupDto>> QueryAsync(
        QueryConfiguration<GroupQueryParameter> configuration,
        CancellationToken cancellationToken = default);
}