using AutoMapper;
using Kysect.Shreks.Application.Dto.Study;
using Kysect.Shreks.Core.Study;
using Kysect.Shreks.DataAccess.Abstractions;
using Kysect.Shreks.DataAccess.Abstractions.Extensions;
using MediatR;
using static Kysect.Shreks.Application.Abstractions.Study.Commands.UpdateSubjectCourse;

namespace Kysect.Shreks.Application.Handlers.Study;

public class UpdateSubjectCourseHandler : IRequestHandler<Command, Response>
{
    private readonly IShreksDatabaseContext _context;
    private readonly IMapper _mapper;

    public UpdateSubjectCourseHandler(IShreksDatabaseContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
    {
        SubjectCourse subjectCourse = await _context.SubjectCourses.GetByIdAsync(request.Id, cancellationToken: cancellationToken);
        subjectCourse.Title = request.NewTitle;
        await _context.SaveChangesAsync(cancellationToken);
        return new Response(_mapper.Map<SubjectCourseDto>(subjectCourse));
    }
}