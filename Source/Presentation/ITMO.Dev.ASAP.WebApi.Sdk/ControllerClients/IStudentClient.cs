using ITMO.Dev.ASAP.Application.Dto.Querying;
using ITMO.Dev.ASAP.Application.Dto.Users;
using ITMO.Dev.ASAP.WebApi.Abstractions.Models.Students;

namespace ITMO.Dev.ASAP.WebApi.Sdk.ControllerClients;

public interface IStudentClient
{
    Task<StudentDto> CreateAsync(CreateStudentRequest request, CancellationToken cancellationToken = default);

    Task<StudentDto> GetAsync(Guid id, CancellationToken cancellationToken = default);

    Task DismissFromGroupAsync(Guid id, CancellationToken cancellationToken = default);

    Task<StudentDto> TransferStudentAsync(
        Guid id,
        TransferStudentRequest request,
        CancellationToken cancellationToken = default);

    Task AddGithubAssociationAsync(Guid id, string githubUsername, CancellationToken cancellationToken = default);

    Task RemoveGithubAssociationAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<StudentDto>> QueryAsync(
        QueryConfiguration<StudentQueryParameter> configuration,
        CancellationToken cancellationToken = default);
}