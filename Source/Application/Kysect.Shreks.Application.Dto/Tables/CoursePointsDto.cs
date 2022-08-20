using Kysect.Shreks.Application.Dto.Study;

namespace Kysect.Shreks.Application.Dto.Tables;

public record struct CoursePointsDto(
    IReadOnlyCollection<AssignmentDto> Assignments,
    IReadOnlyCollection<StudentPointsDto> StudentsPoints);