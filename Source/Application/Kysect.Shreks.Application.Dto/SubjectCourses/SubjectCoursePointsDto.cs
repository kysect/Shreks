using Kysect.Shreks.Application.Dto.Study;
using Kysect.Shreks.Application.Dto.Tables;

namespace Kysect.Shreks.Application.Dto.SubjectCourses;

public record struct SubjectCoursePointsDto(
    IReadOnlyCollection<AssignmentDto> Assignments,
    IReadOnlyList<StudentPointsDto> StudentsPoints);