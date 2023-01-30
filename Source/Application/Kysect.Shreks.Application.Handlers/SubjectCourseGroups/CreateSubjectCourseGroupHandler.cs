using Kysect.Shreks.Core.Study;
using Kysect.Shreks.DataAccess.Abstractions;
using Kysect.Shreks.DataAccess.Abstractions.Extensions;
using Kysect.Shreks.Mapping.Mappings;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static Kysect.Shreks.Application.Contracts.SubjectCourseGroups.Commands.CreateSubjectCourseGroup;

namespace Kysect.Shreks.Application.Handlers.SubjectCourseGroups;

internal class CreateSubjectCourseGroupHandler : IRequestHandler<Command, Response>
{
    private readonly IShreksDatabaseContext _context;

    public CreateSubjectCourseGroupHandler(IShreksDatabaseContext context)
    {
        _context = context;
    }

    public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
    {
        SubjectCourse subjectCourse = await _context.SubjectCourses
            .Include(x => x.Assignments)
            .ThenInclude(x => x.GroupAssignments)
            .GetByIdAsync(request.SubjectCourseId, cancellationToken);

        StudentGroup studentGroup = await _context.StudentGroups
            .GetByIdAsync(request.StudentGroupId, cancellationToken);

        SubjectCourseGroup subjectCourseGroup = subjectCourse.AddGroup(studentGroup);

        foreach (Assignment assignment in subjectCourse.Assignments)
        {
            assignment.AddGroup(studentGroup, DateOnly.FromDateTime(DateTime.UnixEpoch));
        }

        await _context.SubjectCourseGroups.AddAsync(subjectCourseGroup, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return new Response(subjectCourseGroup.ToDto());
    }
}