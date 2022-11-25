using AutoMapper;
using Kysect.Shreks.Application.Dto.SubjectCourses;
using Kysect.Shreks.DataAccess.Abstractions;
using Kysect.Shreks.DataAccess.Abstractions.Extensions;
using MediatR;
using static Kysect.Shreks.Application.Contracts.Study.Commands.CreateSubjectCourseGroup;

namespace Kysect.Shreks.Application.Handlers.Study.SubjectCourseGroup;

public class CreateSubjectCourseGroupHandler : IRequestHandler<Command, Response>
{
    private readonly IShreksDatabaseContext _context;
    private readonly IMapper _mapper;

    public CreateSubjectCourseGroupHandler(IShreksDatabaseContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
    {
        var subjectCourse = await _context.SubjectCourses.GetByIdAsync(request.SubjectCourseId, cancellationToken);

        var studentGroup = await _context.StudentGroups.GetByIdAsync(request.StudentGroupId, cancellationToken);

        var subjectCourseGroup = subjectCourse.AddGroup(studentGroup);

        await _context.SubjectCourseGroups.AddAsync(subjectCourseGroup, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        var dto = _mapper.Map<SubjectCourseGroupDto>(subjectCourseGroup);

        return new Response(dto);
    }
}