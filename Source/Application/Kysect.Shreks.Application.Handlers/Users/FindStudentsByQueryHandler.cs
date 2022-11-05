using AutoMapper;
using Kysect.Shreks.Application.Dto.Users;
using Kysect.Shreks.Application.Queries;
using Kysect.Shreks.Core.Users;
using Kysect.Shreks.DataAccess.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static Kysect.Shreks.Application.Abstractions.Users.Queries.FindStudentsByQuery;

namespace Kysect.Shreks.Application.Handlers.Users;

internal class FindStudentsByQueryHandler : IRequestHandler<Query, Response>
{
    private readonly IEntityQuery<Student, StudentQueryParameter> _query;
    private readonly IShreksDatabaseContext _context;
    private readonly IMapper _mapper;

    public FindStudentsByQueryHandler(
        IEntityQuery<Student, StudentQueryParameter> query,
        IShreksDatabaseContext context,
        IMapper mapper)
    {
        _query = query;
        _context = context;
        _mapper = mapper;
    }

    public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
    {
        IQueryable<Student> query = _context.Students;
        query = _query.Apply(query, request.Configuration);

        List<Student> students = await query.ToListAsync(cancellationToken);
        StudentDto[] dto = students.Select(_mapper.Map<StudentDto>).ToArray();

        return new Response(dto);
    }
}