namespace Kysect.Shreks.Controllers.Models;

public record LoginResponse(string Token, DateTime Expires, IReadOnlyCollection<string> Roles);