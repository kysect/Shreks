using AutoMapper;
using Kysect.Shreks.Application.Dto.Study;
using Kysect.Shreks.Common.Exceptions;
using Kysect.Shreks.DataAccess.Abstractions;
using Kysect.Shreks.DataAccess.Abstractions.Extensions;
using MediatR;
using static Kysect.Shreks.Application.Abstractions.Study.Commands.DeleteSubjectCourseGroup;

namespace Kysect.Shreks.Application.Handlers.Study.SubjectCourseGroup;

public class DeleteSubjectCourseGroupHandler : IRequestHandler<Command>
{
    private readonly IShreksDatabaseContext _context;
    private readonly IMapper _mapper;

    public DeleteSubjectCourseGroupHandler(IShreksDatabaseContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
    {
        var subjectCourse = await _context.SubjectCourses.GetByIdAsync(request.SubjectCourseId, cancellationToken);

        var subjectCourseGroup = subjectCourse.Groups.FirstOrDefault(g => g.StudentGroupId == request.StudentGroupId);

        if (subjectCourseGroup is null)
            throw new EntityNotFoundException($"SubjectCourseGroup with id {request.StudentGroupId} not found");

        subjectCourse.RemoveGroup(subjectCourseGroup);

        _context.SubjectCourseGroups.Update(subjectCourseGroup);
        await _context.SaveChangesAsync(cancellationToken);


        return Unit.Value;
    }
}