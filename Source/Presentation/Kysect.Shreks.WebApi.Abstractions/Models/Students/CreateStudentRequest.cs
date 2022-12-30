namespace Kysect.Shreks.WebApi.Abstractions.Models.Students;

public record CreateStudentRequest(string? FirstName, string? MiddleName, string? LastName, Guid GroupId);