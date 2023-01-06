using Kysect.Shreks.WebApi.Abstractions.Models.Identity;

namespace Kysect.Shreks.WebApi.Sdk.ControllerClients;

public interface IIdentityClient
{
    Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);

    Task PromoteAsync(string username, CancellationToken cancellationToken = default);

    Task<LoginResponse> RegisterAsync(RegisterUserRequest request, CancellationToken cancellationToken = default);
}