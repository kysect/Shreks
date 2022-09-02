using AutoMapper;
using Kysect.Shreks.Core.Users;
using Kysect.Shreks.DataAccess.Abstractions;
using Kysect.Shreks.DataAccess.Abstractions.Extensions;
using MediatR;
using static Kysect.Shreks.Application.Abstractions.Students.DeleteStudent;

namespace Kysect.Shreks.Application.Handlers.Students;

public class DeleteStudentHandler : IRequestHandler<Command, Response>
{
    private readonly IShreksDatabaseContext _context;

    public DeleteStudentHandler(IShreksDatabaseContext context, IMapper mapper)
    {
        _context = context;
    }

    public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
    {
        Student student = await _context.Students.GetByIdAsync(request.Id, cancellationToken);

        _context.Students.Remove(student);
        await _context.SaveChangesAsync(cancellationToken);

        return new Response();
    }
}