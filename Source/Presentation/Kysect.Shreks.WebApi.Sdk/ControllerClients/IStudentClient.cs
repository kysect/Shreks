using Kysect.Shreks.Application.Dto.Querying;
using Kysect.Shreks.Application.Dto.Users;
using Kysect.Shreks.WebApi.Abstractions.Models.Students;

namespace Kysect.Shreks.WebApi.Sdk.ControllerClients;

public interface IStudentClient
{
    Task<StudentDto> CreateAsync(CreateStudentRequest request, CancellationToken cancellationToken = default);

    Task<StudentDto> GetAsync(Guid id, CancellationToken cancellationToken = default);

    Task DismissFromGroupAsync(Guid id, CancellationToken cancellationToken = default);

    Task AddGithubAssociationAsync(Guid id, string githubUsername, CancellationToken cancellationToken = default);

    Task RemoveGithubAssociationAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<StudentDto>> QueryAsync(
        QueryConfiguration<StudentQueryParameter> configuration,
        CancellationToken cancellationToken = default);
}