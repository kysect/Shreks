using ITMO.Dev.ASAP.Application.Dto.Users;

namespace ITMO.Dev.ASAP.Application.Dto.Tables;

public record StudentPointsDto(StudentDto Student, IReadOnlyCollection<AssignmentPointsDto> Points);