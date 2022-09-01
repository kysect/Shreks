namespace Kysect.Shreks.WebApi.Models;

public record LoginResponse(string Token, DateTime Expires, IReadOnlyCollection<string> Roles);