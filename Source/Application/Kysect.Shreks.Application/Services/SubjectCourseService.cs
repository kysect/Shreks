using AutoMapper;
using Kysect.Shreks.Application.Abstractions.SubjectCourses;
using Kysect.Shreks.Application.Dto.Study;
using Kysect.Shreks.Application.Dto.SubjectCourses;
using Kysect.Shreks.Application.Dto.Tables;
using Kysect.Shreks.Application.Dto.Users;
using Kysect.Shreks.Application.Extensions;
using Kysect.Shreks.Core.Study;
using Kysect.Shreks.Core.Users;
using Kysect.Shreks.DataAccess.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Kysect.Shreks.Application.Services;

public class SubjectCourseService : ISubjectCourseService
{
    private readonly IShreksDatabaseContext _context;
    private readonly IMapper _mapper;

    public SubjectCourseService(IShreksDatabaseContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
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
            .ToListAsync(cancellationToken);

        IEnumerable<StudentAssignment> studentAssignmentPoints = assignments
            .SelectMany(x => x.GroupAssignments)
            .SelectMany(ga => ga.Group.Students.Select(s => new StudentAssignment(s, ga)));

        StudentPointsDto[] studentPoints = studentAssignmentPoints
            .GroupBy(x => x.Student)
            .Select(MapToStudentPoints)
            .ToArray();

        AssignmentDto[] assignmentsDto = assignments.Select(_mapper.Map<AssignmentDto>).ToArray();
        return new SubjectCoursePointsDto(assignmentsDto, studentPoints);
    }

    private StudentPointsDto MapToStudentPoints(IGrouping<Student, StudentAssignment> grouping)
    {
        StudentDto studentDto = _mapper.Map<StudentDto>(grouping.Key);

        AssignmentPointsDto[] pointsDto = grouping
            .Select(x => x.Points)
            .WhereNotNull()
            .Select(x => new AssignmentPointsDto(x.Assignment.Id, x.SubmissionDate, x.IsBanned, x.Points.Value))
            .ToArray();

        return new StudentPointsDto(studentDto, pointsDto);
    }
}