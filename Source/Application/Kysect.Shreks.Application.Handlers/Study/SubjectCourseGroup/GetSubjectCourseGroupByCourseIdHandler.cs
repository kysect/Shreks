using AutoMapper;
using Kysect.Shreks.Application.Dto.SubjectCourses;
using Kysect.Shreks.DataAccess.Abstractions;
using Kysect.Shreks.DataAccess.Abstractions.Extensions;
using MediatR;
using static Kysect.Shreks.Application.Contracts.Study.Queries.GetSubjectCourseGroupsBySubjectCourseId;

namespace Kysect.Shreks.Application.Handlers.Study.SubjectCourseGroup;

public class GetSubjectCourseGroupsByIdHandler : IRequestHandler<Query, Response>
{
    private readonly IShreksDatabaseContext _context;
    private readonly IMapper _mapper;

    public GetSubjectCourseGroupsByIdHandler(IShreksDatabaseContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
    {
        var subjectCourse = await _context.SubjectCourses.GetByIdAsync(request.SubjectCourseId, cancellationToken);

        var subjectCourseGroups = subjectCourse.Groups.Select(_mapper.Map<SubjectCourseGroupDto>).ToList();

        return new Response(subjectCourseGroups);
    }
}