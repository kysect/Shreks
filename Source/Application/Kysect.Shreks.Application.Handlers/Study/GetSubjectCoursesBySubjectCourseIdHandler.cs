using AutoMapper;
using Kysect.Shreks.Application.Dto.SubjectCourses;
using Kysect.Shreks.Core.Study;
using Kysect.Shreks.DataAccess.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static Kysect.Shreks.Application.Abstractions.Study.Queries.GetSubjectCoursesBySubjectCourseId;

namespace Kysect.Shreks.Application.Handlers.Study;

internal class GetSubjectCoursesBySubjectCourseIdHandler : IRequestHandler<Query, Response>
{
    private readonly IShreksDatabaseContext _context;
    private readonly IMapper _mapper;

    public GetSubjectCoursesBySubjectCourseIdHandler(IShreksDatabaseContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
    {
        List<SubjectCourse> courses = await _context.SubjectCourses
            .Where(x => x.Subject.Id.Equals(request.SubjectCourseId))
            .ToListAsync(cancellationToken);

        SubjectCourseDto[] dto = courses
            .Select(_mapper.Map<SubjectCourseDto>)
            .ToArray();
        
        return new Response(dto);
    }
}