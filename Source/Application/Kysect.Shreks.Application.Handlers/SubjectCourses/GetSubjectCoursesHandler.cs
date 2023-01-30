using AutoMapper;
using Kysect.Shreks.Application.Dto.SubjectCourses;
using Kysect.Shreks.Core.Study;
using Kysect.Shreks.DataAccess.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static Kysect.Shreks.Application.Contracts.Study.Queries.GetSubjectCourses;

namespace Kysect.Shreks.Application.Handlers.SubjectCourses;

internal class GetSubjectCoursesHandler : IRequestHandler<Query, Response>
{
    private readonly IShreksDatabaseContext _context;
    private readonly IMapper _mapper;

    public GetSubjectCoursesHandler(IShreksDatabaseContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
    {
        List<SubjectCourse> subjectCourse = await _context
            .SubjectCourses
            .ToListAsync(cancellationToken);

        SubjectCourseDto[] dto = subjectCourse
            .Select(_mapper.Map<SubjectCourseDto>)
            .ToArray();

        return new Response(dto);
    }
}