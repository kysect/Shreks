namespace ITMO.Dev.ASAP.Application.Dto.Users;

public record StudentDto(UserDto User, string GroupName, int? UniversityId, string? GitHubUsername);