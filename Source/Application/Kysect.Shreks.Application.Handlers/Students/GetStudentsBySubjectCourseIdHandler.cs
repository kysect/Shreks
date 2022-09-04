using AutoMapper;
using AutoMapper.QueryableExtensions;
using Kysect.Shreks.Application.Dto.Users;
using Kysect.Shreks.DataAccess.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static Kysect.Shreks.Application.Abstractions.Students.GetStudentsBySubjectCourseId;

namespace Kysect.Shreks.Application.Handlers.Students;

public class GetStudentsBySubjectCourseIdHandler : IRequestHandler<Query, Response>
{
    private readonly IShreksDatabaseContext _context;
    private readonly IMapper _mapper;

    public GetStudentsBySubjectCourseIdHandler(IShreksDatabaseContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
    {
        List<StudentDto> students = await _context
            .SubjectCourses
            .Where(sc => sc.Id == request.SubjectCourseId)
            .SelectMany(sc => sc.Groups)
            .SelectMany(sg => sg.StudentGroup.Students)
            .ProjectTo<StudentDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

        return new Response(students);
    }
}