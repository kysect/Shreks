using Kysect.Shreks.Common.Exceptions;
using Kysect.Shreks.Core.Users;
using Kysect.Shreks.DataAccess.Abstractions;
using Kysect.Shreks.Mapping.Mappings;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static Kysect.Shreks.Application.Contracts.Students.Queries.GetStudentById;

namespace Kysect.Shreks.Application.Handlers.Students;

internal class GetStudentByIdHandler : IRequestHandler<Query, Response>
{
    private readonly IShreksDatabaseContext _context;

    public GetStudentByIdHandler(IShreksDatabaseContext context)
    {
        _context = context;
    }

    public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
    {
        Student? student = await _context.Students
            .Where(s => s.User.Id == request.UserId)
            .FirstOrDefaultAsync(cancellationToken);

        if (student == default)
            throw new EntityNotFoundException($"Student not found for user {request.UserId}");

        return new Response(student.ToDto());
    }
}