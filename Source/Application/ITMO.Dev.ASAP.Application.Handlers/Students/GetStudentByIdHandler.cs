using ITMO.Dev.ASAP.Common.Exceptions;
using ITMO.Dev.ASAP.Core.Users;
using ITMO.Dev.ASAP.DataAccess.Abstractions;
using ITMO.Dev.ASAP.Mapping.Mappings;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static ITMO.Dev.ASAP.Application.Contracts.Students.Queries.GetStudentById;

namespace ITMO.Dev.ASAP.Application.Handlers.Students;

internal class GetStudentByIdHandler : IRequestHandler<Query, Response>
{
    private readonly IDatabaseContext _context;

    public GetStudentByIdHandler(IDatabaseContext context)
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