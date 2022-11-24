using AutoMapper;
using Kysect.Shreks.Application.Dto.Study;
using Kysect.Shreks.Application.Dto.SubjectCourses;
using Kysect.Shreks.Application.Dto.Tables;
using Kysect.Shreks.Application.Dto.Users;
using Kysect.Shreks.Application.Extensions;
using Kysect.Shreks.Core.Study;
using Kysect.Shreks.Core.Users;
using Kysect.Shreks.DataAccess.Abstractions;
using Kysect.Shreks.DataAccess.Abstractions.Extensions;
using MediatR;
using Microsoft.Extensions.Logging;
using static Kysect.Shreks.Application.Abstractions.SubjectCourses.Queries.GetSubjectCoursePoints;

namespace Kysect.Shreks.Application.Handlers.SubjectCourses;

public class GetSubjectCoursePointsHandler : IRequestHandler<Query, Response>
{
    private readonly IShreksDatabaseContext _context;
    private readonly IMapper _mapper;
    private readonly ILogger<GetSubjectCoursePointsHandler> _logger;

    public GetSubjectCoursePointsHandler(
        IShreksDatabaseContext context,
        IMapper mapper,
        ILogger<GetSubjectCoursePointsHandler> logger)
    {
        _context = context;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        _logger.LogInformation("Started to collecting all course {courseId} points", request.SubjectCourseId);

        SubjectCourse subjectCourse = await _context.SubjectCourses
            .GetByIdAsync(request.SubjectCourseId, cancellationToken);

        IEnumerable<StudentAssignment> studentAssignmentPoints = subjectCourse.Assignments
            .SelectMany(x => x.GroupAssignments)
            .SelectMany(ga => ga.Group.Students.Select(s => new StudentAssignment(s, ga)));

        StudentPointsDto[] studentPoints = studentAssignmentPoints
            .GroupBy(x => x.Student)
            .Select(MapToStudentPoints)
            .ToArray();

        AssignmentDto[] assignmentsDto = subjectCourse.Assignments
            .Select(_mapper.Map<AssignmentDto>)
            .ToArray();

        _logger.LogInformation("Finished to collect all course {courseId} points", request.SubjectCourseId);

        var points = new SubjectCoursePointsDto(assignmentsDto, studentPoints);
        return new Response(points);
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