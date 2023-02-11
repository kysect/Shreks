using ITMO.Dev.ASAP.Application.Dto.Tables;

namespace ITMO.Dev.ASAP.Integration.Google.Models;

public record struct CourseStudentsDto(IReadOnlyList<StudentPointsDto> StudentsPoints);