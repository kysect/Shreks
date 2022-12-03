using AutoMapper;
using Kysect.Shreks.Application.Dto.SubjectCourses;
using Kysect.Shreks.Core.Study;
using Kysect.Shreks.DataAccess.Abstractions;
using Kysect.Shreks.DataAccess.Abstractions.Extensions;
using MediatR;
using static Kysect.Shreks.Application.Contracts.Study.Queries.GetSubjectCourseById;

namespace Kysect.Shreks.Application.Handlers.Study;

internal class GetSubjectCourseByIdHandler : IRequestHandler<Query, Response>
{
    private readonly IShreksDatabaseContext _context;
    private readonly IMapper _mapper;

    public GetSubjectCourseByIdHandler(IShreksDatabaseContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
    {
        SubjectCourse subjectCourse = await _context
            .SubjectCourses
            .GetByIdAsync(request.Id, cancellationToken: cancellationToken);

        return new Response(_mapper.Map<SubjectCourseDto>(subjectCourse));
    }
}