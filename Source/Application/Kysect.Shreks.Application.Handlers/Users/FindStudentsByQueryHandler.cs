using Kysect.Shreks.Application.Dto.Querying;
using Kysect.Shreks.Application.Dto.Users;
using Kysect.Shreks.Application.Queries;
using Kysect.Shreks.Core.Users;
using Kysect.Shreks.DataAccess.Abstractions;
using Kysect.Shreks.Mapping.Mappings;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static Kysect.Shreks.Application.Contracts.Users.Queries.FindStudentsByQuery;

namespace Kysect.Shreks.Application.Handlers.Users;

internal class FindStudentsByQueryHandler : IRequestHandler<Query, Response>
{
    private readonly IShreksDatabaseContext _context;
    private readonly IEntityQuery<Student, StudentQueryParameter> _query;

    public FindStudentsByQueryHandler(
        IEntityQuery<Student, StudentQueryParameter> query,
        IShreksDatabaseContext context)
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