using ITMO.Dev.ASAP.Application.Dto.Users;
using ITMO.Dev.ASAP.Core.Users;
using ITMO.Dev.ASAP.DataAccess.Abstractions;
using ITMO.Dev.ASAP.Mapping.Mappings;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static ITMO.Dev.ASAP.Application.Contracts.Students.Queries.GetStudentsBySubjectCourseId;

namespace ITMO.Dev.ASAP.Application.Handlers.Students;

internal class GetStudentsBySubjectCourseIdHandler : IRequestHandler<Query, Response>
{
    private readonly IDatabaseContext _context;

    public GetStudentsBySubjectCourseIdHandler(IDatabaseContext context)
    {
        _context = context;
    }

    public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
    {
        List<Student> students = await _context
            .SubjectCourses
            .Where(sc => sc.Id == request.SubjectCourseId)
            .SelectMany(sc => sc.Groups)
            .SelectMany(sg => sg.StudentGroup.Students)
            .ToListAsync(cancellationToken);

        StudentDto[] dto = students.Select(x => x.ToDto()).ToArray();

        return new Response(dto);
    }
}