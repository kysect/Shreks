using ITMO.Dev.ASAP.Common.Exceptions;
using ITMO.Dev.ASAP.Core.Users;
using ITMO.Dev.ASAP.DataAccess.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static ITMO.Dev.ASAP.Application.Contracts.Users.Commands.DismissStudentFromGroup;

namespace ITMO.Dev.ASAP.Application.Handlers.Students;

internal class DismissStudentFromGroupHandler : IRequestHandler<Command>
{
    private readonly IDatabaseContext _context;

    public DismissStudentFromGroupHandler(IDatabaseContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
    {
        Student? student = await _context.Students
            .SingleOrDefaultAsync(x => x.User.Id.Equals(request.StudentId), cancellationToken);

        if (student is null)
            throw EntityNotFoundException.For<Student>(request.StudentId);

        student.DismissFromStudyGroup();

        _context.Students.Update(student);
        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}