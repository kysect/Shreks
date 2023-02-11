using ITMO.Dev.ASAP.Common.Exceptions;
using ITMO.Dev.ASAP.Core.Study;
using ITMO.Dev.ASAP.Core.Users;
using ITMO.Dev.ASAP.DataAccess.Abstractions;
using ITMO.Dev.ASAP.Mapping.Mappings;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static ITMO.Dev.ASAP.Application.Contracts.Students.Commands.CreateStudent;

namespace ITMO.Dev.ASAP.Application.Handlers.Students;

internal class CreateStudentHandler : IRequestHandler<Command, Response>
{
    private readonly IDatabaseContext _context;

    public CreateStudentHandler(IDatabaseContext context)
    {
        _context = context;
    }

    public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
    {
        StudentGroup? group = await _context.StudentGroups
            .Include(x => x.Students)
            .SingleOrDefaultAsync(x => x.Id.Equals(request.GroupId), cancellationToken);

        if (group is null)
            throw EntityNotFoundException.For<StudentGroup>(request.GroupId);

        var user = new User(Guid.NewGuid(), request.FirstName, request.MiddleName, request.LastName);
        var student = new Student(user, group);

        _context.Students.Add(student);
        await _context.SaveChangesAsync(cancellationToken);

        return new Response(student.ToDto());
    }
}