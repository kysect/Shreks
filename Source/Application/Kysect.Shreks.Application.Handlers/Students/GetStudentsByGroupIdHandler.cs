using AutoMapper;
using Kysect.Shreks.Application.Dto.Users;
using Kysect.Shreks.Core.Users;
using Kysect.Shreks.DataAccess.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static Kysect.Shreks.Application.Contracts.Students.Queries.GetStudentsByGroupId;

namespace Kysect.Shreks.Application.Handlers.Students;

internal class GetStudentsByGroupIdHandler : IRequestHandler<Query, Response>
{
    private readonly IShreksDatabaseContext _context;
    private readonly IMapper _mapper;

    public GetStudentsByGroupIdHandler(IShreksDatabaseContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
    {
        List<Student> students = await _context.Students
            .Where(s => s.Group != null && s.Group.Id.Equals(request.GroupId))
            .ToListAsync(cancellationToken);

        StudentDto[] dto = students.Select(_mapper.Map<StudentDto>).ToArray();

        return new Response(dto);
    }
}