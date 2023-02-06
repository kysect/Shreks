using Kysect.Shreks.Application.Abstractions.Google;
using Kysect.Shreks.Core.Study;
using Kysect.Shreks.DataAccess.Abstractions;
using Kysect.Shreks.DataAccess.Abstractions.Extensions;
using Kysect.Shreks.Mapping.Mappings;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static Kysect.Shreks.Application.Contracts.SubjectCourseGroups.Commands.CreateSubjectCourseGroup;

namespace Kysect.Shreks.Application.Handlers.Study.SubjectCourseGroups;

internal class CreateSubjectCourseGroupHandler : IRequestHandler<Command, Response>
{
    private readonly IShreksDatabaseContext _context;
    private readonly ITableUpdateQueue _tableUpdateQueue;

    public CreateSubjectCourseGroupHandler(IShreksDatabaseContext context, ITableUpdateQueue tableUpdateQueue)
    {
        _context = context;
        _tableUpdateQueue = tableUpdateQueue;
    }

    public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
    {
        SubjectCourse subjectCourse = await _context.SubjectCourses
            .Include(x => x.Assignments)
            .ThenInclude(x => x.GroupAssignments)
            .GetByIdAsync(request.SubjectCourseId, cancellationToken);

        StudentGroup studentGroup = await _context.StudentGroups
            .GetByIdAsync(request.StudentGroupId, cancellationToken);

        SubjectCourseGroup group = subjectCourse.AddGroup(studentGroup);

        await _context.SubjectCourseGroups.AddAsync(group, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        _tableUpdateQueue.EnqueueSubmissionsQueueUpdate(group.SubjectCourseId, group.StudentGroupId);

        return new Response(group.ToDto());
    }
}