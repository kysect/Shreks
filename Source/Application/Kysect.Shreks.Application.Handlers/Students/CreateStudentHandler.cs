using AutoMapper;
using Kysect.Shreks.Application.Dto.Users;
using Kysect.Shreks.Common.Exceptions;
using Kysect.Shreks.Core.Study;
using Kysect.Shreks.Core.Users;
using Kysect.Shreks.DataAccess.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static Kysect.Shreks.Application.Contracts.Students.Commands.CreateStudent;

namespace Kysect.Shreks.Application.Handlers.Students;

internal class CreateStudentHandler : IRequestHandler<Command, Response>
{
    private readonly IShreksDatabaseContext _context;
    private readonly IMapper _mapper;

    public CreateStudentHandler(IShreksDatabaseContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
    {
        StudentGroup? group = await _context.StudentGroups
            .Include(x => x.Students)
            .SingleOrDefaultAsync(x => x.Id.Equals(request.GroupId), cancellationToken);

        if (group is null)
            throw EntityNotFoundException.For<StudentGroup>(request.GroupId);

        var user = new User(request.FirstName, request.MiddleName, request.LastName);
        var student = new Student(user, group);

        _context.Students.Add(student);
        await _context.SaveChangesAsync(cancellationToken);

        StudentDto dto = _mapper.Map<StudentDto>(student);

        return new Response(dto);
    }
}