using Kysect.Shreks.Application.Dto.Users;

namespace Kysect.Shreks.Integration.Google.Models;

public record struct CourseStudentsDto(IReadOnlyList<StudentDto> Students);