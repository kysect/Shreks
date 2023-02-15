using ITMO.Dev.ASAP.Application.Abstractions.Formatters;
using ITMO.Dev.ASAP.Application.Abstractions.SubjectCourses;
using ITMO.Dev.ASAP.Application.Dto.Study;
using ITMO.Dev.ASAP.Application.Dto.SubjectCourses;
using ITMO.Dev.ASAP.Application.Dto.Tables;
using ITMO.Dev.ASAP.Application.Dto.Users;
using ITMO.Dev.ASAP.Application.Extensions;
using ITMO.Dev.ASAP.Core.Study;
using ITMO.Dev.ASAP.Core.Users;
using ITMO.Dev.ASAP.DataAccess.Abstractions;
using ITMO.Dev.ASAP.Mapping.Mappings;
using Microsoft.EntityFrameworkCore;

namespace ITMO.Dev.ASAP.Application.Services;

public class SubjectCourseService : ISubjectCourseService
{
    private readonly IDatabaseContext _context;
    private readonly IUserFullNameFormatter _userFullNameFormatter;

    public SubjectCourseService(IDatabaseContext context, IUserFullNameFormatter userFullNameFormatter)
    {
        _context = context;
        _userFullNameFormatter = userFullNameFormatter;
    }

    public async Task<SubjectCoursePointsDto> CalculatePointsAsync(
        Guid subjectCourseId,
        CancellationToken cancellationToken)
    {
        List<Assignment> assignments = await _context.Assignments
            .Include(x => x.GroupAssignments)
            .ThenInclude(x => x.Group)
            .ThenInclude(x => x.Students)
            .ThenInclude(x => x.User)
            .ThenInclude(x => x.Associations)
            .Include(x => x.GroupAssignments)
            .ThenInclude(x => x.Submissions)
            .AsSplitQuery()
            .Where(x => x.SubjectCourse.Id.Equals(subjectCourseId))
            .OrderBy(x => x.Order)
            .ToListAsync(cancellationToken);

        IEnumerable<StudentAssignment> studentAssignmentPoints = assignments
            .SelectMany(x => x.GroupAssignments)
            .SelectMany(ga => ga.Group.Students.Select(s => new StudentAssignment(s, ga)));

        StudentPointsDto[] studentPoints = studentAssignmentPoints
            .GroupBy(x => x.Student)
            .Select(MapToStudentPoints)
            .OrderBy(x => x.Student.GroupName)
            .ThenBy(x => _userFullNameFormatter.GetFullName(x.Student.User))
            .ToArray();

        AssignmentDto[] assignmentsDto = assignments.Select(x => x.ToDto()).ToArray();
        return new SubjectCoursePointsDto(assignmentsDto, studentPoints);
    }

    private StudentPointsDto MapToStudentPoints(IGrouping<Student, StudentAssignment> grouping)
    {
        StudentDto studentDto = grouping.Key.ToDto();

        AssignmentPointsDto[] pointsDto = grouping
            .Select(x => x.Points)
            .WhereNotNull()
            .Select(x => new AssignmentPointsDto(x.Assignment.Id, x.SubmissionDate, x.IsBanned, x.Points.Value))
            .ToArray();

        return new StudentPointsDto(studentDto, pointsDto);
    }
}