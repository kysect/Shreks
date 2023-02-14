using ITMO.Dev.ASAP.Application.Dto.Users;
using ITMO.Dev.ASAP.Core.Users;
using ITMO.Dev.ASAP.DataAccess.Abstractions;
using ITMO.Dev.ASAP.Mapping.Mappings;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static ITMO.Dev.ASAP.Application.Contracts.Students.Queries.GetStudentsByGroupId;

namespace ITMO.Dev.ASAP.Application.Handlers.Students;

internal class GetStudentsByGroupIdHandler : IRequestHandler<Query, Response>
{
    private readonly IDatabaseContext _context;

    public GetStudentsByGroupIdHandler(IDatabaseContext context)
    {
        _context = context;
    }

    public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
    {
        List<Student> students = await _context.Students
            .Where(s => s.Group != null && s.Group.Id.Equals(request.GroupId))
            .ToListAsync(cancellationToken);

        StudentDto[] dto = students.Select(x => x.ToDto()).ToArray();

        return new Response(dto);
    }
}