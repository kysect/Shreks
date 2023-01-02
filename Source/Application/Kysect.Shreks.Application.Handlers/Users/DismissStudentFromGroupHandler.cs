using Kysect.Shreks.Common.Exceptions;
using Kysect.Shreks.Core.Users;
using Kysect.Shreks.DataAccess.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static Kysect.Shreks.Application.Contracts.Users.Commands.DismissStudentFromGroup;

namespace Kysect.Shreks.Application.Handlers.Users;

internal class DismissStudentFromGroupHandler : IRequestHandler<Command>
{
    private readonly IShreksDatabaseContext _context;

    public DismissStudentFromGroupHandler(IShreksDatabaseContext context)
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