using Kysect.Shreks.Application.Abstractions.Exceptions;
using Kysect.Shreks.DataAccess.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static Kysect.Shreks.Application.Abstractions.Students.GetStudentByUser;

namespace Kysect.Shreks.Application.Handlers.Students;

public class GetStudentByUserHandler : IRequestHandler<Query, Response>
{

    private readonly IShreksDatabaseContext _context;

    public GetStudentByUserHandler(IShreksDatabaseContext context)
    {
        _context = context;
    }

    public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
    {
        var studentId = await _context.Students
            .Where(s => s.User.Id == request.UserId)
            .Select(s => s.UserId)
            .FirstOrDefaultAsync(cancellationToken);

        if (studentId == default)
        {
            throw new EntityNotFoundException($"Student not found for user {request.UserId}");
        }

        return new Response(studentId);
    }
}