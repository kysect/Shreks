using AutoMapper;
using Kysect.Shreks.Application.Dto.Users;
using Kysect.Shreks.Core.Study;
using Kysect.Shreks.Core.Users;
using Kysect.Shreks.DataAccess.Abstractions;
using Kysect.Shreks.DataAccess.Abstractions.Extensions;
using MediatR;
using static Kysect.Shreks.Application.Abstractions.Students.CreateStudent;

namespace Kysect.Shreks.Application.Handlers.Students;

public class CreateStudentHandler : IRequestHandler<Command, Response>
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
        StudentGroup group = await _context.StudentGroups.GetByIdAsync(request.GroupId, cancellationToken);

        var user = new User(request.FirstName, request.MiddleName, request.LastName);
        var student = new Student(user, group, request.UniversityId);

        _context.Students.Add(student);
        await _context.SaveChangesAsync(cancellationToken);

        var dto = _mapper.Map<StudentDto>(student);

        return new Response(dto);
    }
}