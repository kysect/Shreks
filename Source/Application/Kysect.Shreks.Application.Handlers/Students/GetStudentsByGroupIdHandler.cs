using AutoMapper;
using AutoMapper.QueryableExtensions;
using Kysect.Shreks.Application.Dto.Users;
using Kysect.Shreks.DataAccess.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static Kysect.Shreks.Application.Contracts.Students.Queries.GetStudentsByGroupId;

namespace Kysect.Shreks.Application.Handlers.Students;

public class GetStudentsByGroupIdHandler : IRequestHandler<Query, Response>
{
    private readonly IShreksDatabaseContext _context;
    private readonly IMapper _mapper;

    public GetStudentsByGroupIdHandler(IShreksDatabaseContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
    {
        List<StudentDto> students = await _context.Students
            .Where(s => s.Group.Id == request.GroupId)
            .ProjectTo<StudentDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

        return new Response(students);
    }
}