using ITMO.Dev.ASAP.Application.Dto.Study;
using ITMO.Dev.ASAP.Application.Dto.Tables;

namespace ITMO.Dev.ASAP.Application.Dto.SubjectCourses;

public record struct SubjectCoursePointsDto(
    IReadOnlyCollection<AssignmentDto> Assignments,
    IReadOnlyList<StudentPointsDto> StudentsPoints);