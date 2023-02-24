using ITMO.Dev.ASAP.Application.Dto.Users;

namespace ITMO.Dev.ASAP.WebApi.Sdk.ControllerClients;

public interface IUserClient
{
    Task UpdateUniversityIdAsync(Guid userId, int universityId, CancellationToken cancellationToken = default);

    Task<UserDto?> FindUserByUniversityIdAsync(int universityId, CancellationToken cancellationToken = default);

    Task UpdateNameAsync(
        Guid userId,
        string firstName,
        string middleName,
        string lastName,
        CancellationToken cancellationToken = default);
}