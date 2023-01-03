using Kysect.Shreks.Common.Exceptions;
using Kysect.Shreks.Core.Study;
using Kysect.Shreks.DataAccess.Abstractions;
using Kysect.Shreks.DataAccess.Abstractions.Extensions;
using MediatR;
using static Kysect.Shreks.Application.Contracts.Study.Commands.DeleteSubjectCourseGroup;

namespace Kysect.Shreks.Application.Handlers.Study.SubjectCourseGroup;

internal class DeleteSubjectCourseGroupHandler : IRequestHandler<Command>
{
    private readonly IShreksDatabaseContext _context;

    public DeleteSubjectCourseGroupHandler(IShreksDatabaseContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
    {
        SubjectCourse subjectCourse =
            await _context.SubjectCourses.GetByIdAsync(request.SubjectCourseId, cancellationToken);

        Core.Study.SubjectCourseGroup? subjectCourseGroup =
            subjectCourse.Groups.FirstOrDefault(g => g.StudentGroupId == request.StudentGroupId);

        if (subjectCourseGroup is null)
            throw new EntityNotFoundException($"SubjectCourseGroup with id {request.StudentGroupId} not found");

        subjectCourse.RemoveGroup(subjectCourseGroup);

        _context.SubjectCourseGroups.Update(subjectCourseGroup);
        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}