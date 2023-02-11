namespace ITMO.Dev.ASAP.WebApi.Abstractions.Models.Students;

public record CreateStudentRequest(string? FirstName, string? MiddleName, string? LastName, Guid GroupId);