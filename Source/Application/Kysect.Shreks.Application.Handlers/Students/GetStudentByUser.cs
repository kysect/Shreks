using Kysect.Shreks.Application.Abstractions.DataAccess;
using Kysect.Shreks.Application.Abstractions.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static Kysect.Shreks.Application.Abstractions.Students.GetStudentByUsername;

namespace Kysect.Shreks.Application.Handlers.Students;

public class GetStudentByUser : IRequestHandler<Query, Response>
{

    private readonly IShreksDatabaseContext _context;

    public GetStudentByUser(IShreksDatabaseContext context)
    {
        _context = context;
    }

    public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
    {
        var studentId = await _context.Students
            .Where(s => s.User.Id == request.UserId)
            .Select(s => s.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (studentId == default)
        {
            throw new EntityNotFoundException($"Student not found for user {request.UserId}");
        }

        return new Response(studentId);
    }
}