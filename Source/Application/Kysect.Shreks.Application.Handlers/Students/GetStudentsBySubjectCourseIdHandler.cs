using Kysect.Shreks.Application.Dto.Users;
using Kysect.Shreks.Core.Users;
using Kysect.Shreks.DataAccess.Abstractions;
using Kysect.Shreks.Mapping.Mappings;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static Kysect.Shreks.Application.Contracts.Students.Queries.GetStudentsBySubjectCourseId;

namespace Kysect.Shreks.Application.Handlers.Students;

internal class GetStudentsBySubjectCourseIdHandler : IRequestHandler<Query, Response>
{
    private readonly IShreksDatabaseContext _context;

    public GetStudentsBySubjectCourseIdHandler(IShreksDatabaseContext context)
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