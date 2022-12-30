namespace Kysect.Shreks.WebApi.Abstractions.Models.Identity;

public record LoginResponse(string Token, DateTime Expires, IReadOnlyCollection<string> Roles);