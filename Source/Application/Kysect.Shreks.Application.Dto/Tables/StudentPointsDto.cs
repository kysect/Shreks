using Kysect.Shreks.Application.Dto.Users;

namespace Kysect.Shreks.Application.Dto.Tables;

public record StudentPointsDto(StudentDto Student, IReadOnlyCollection<AssignmentPointsDto> Points);