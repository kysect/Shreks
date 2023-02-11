using ITMO.Dev.ASAP.Application.Dto.Querying;
using ITMO.Dev.ASAP.Application.Dto.Users;
using ITMO.Dev.ASAP.Application.Queries;
using ITMO.Dev.ASAP.Core.Users;
using ITMO.Dev.ASAP.DataAccess.Abstractions;
using ITMO.Dev.ASAP.Mapping.Mappings;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static ITMO.Dev.ASAP.Application.Contracts.Users.Queries.FindStudentsByQuery;

namespace ITMO.Dev.ASAP.Application.Handlers.Students;

internal class FindStudentsByQueryHandler : IRequestHandler<Query, Response>
{
    private readonly IDatabaseContext _context;
    private readonly IEntityQuery<Student, StudentQueryParameter> _query;

    public FindStudentsByQueryHandler(
        IEntityQuery<Student, StudentQueryParameter> query,
        IDatabaseContext context)
    {
        _query = query;
        _context = context;
    }

    public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
    {
        IQueryable<Student> query = _context.Students;
        query = _query.Apply(query, request.Configuration);

        List<Student> students = await query.ToListAsync(cancellationToken);
        StudentDto[] dto = students.Select(x => x.ToDto()).ToArray();

        return new Response(dto);
    }
}