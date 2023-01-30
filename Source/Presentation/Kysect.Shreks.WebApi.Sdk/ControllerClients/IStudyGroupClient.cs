using Kysect.Shreks.Application.Dto.Querying;
using Kysect.Shreks.Application.Dto.Study;
using Kysect.Shreks.Application.Dto.Users;
using Kysect.Shreks.WebApi.Abstractions.Models.StudyGroups;

namespace Kysect.Shreks.WebApi.Sdk.ControllerClients;

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