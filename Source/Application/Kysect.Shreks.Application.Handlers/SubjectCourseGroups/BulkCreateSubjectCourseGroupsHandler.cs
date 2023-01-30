using Kysect.Shreks.Application.Dto.SubjectCourses;
using Kysect.Shreks.Common.Exceptions;
using Kysect.Shreks.Core.Study;
using Kysect.Shreks.DataAccess.Abstractions;
using Kysect.Shreks.Mapping.Mappings;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static Kysect.Shreks.Application.Contracts.SubjectCourseGroups.Commands.BulkCreateSubjectCourseGroups;

namespace Kysect.Shreks.Application.Handlers.SubjectCourseGroups;

internal class BulkCreateSubjectCourseGroupsHandler : IRequestHandler<Command, Response>
{
    private readonly IShreksDatabaseContext _context;

    public BulkCreateSubjectCourseGroupsHandler(IShreksDatabaseContext context)
    {
        _context = context;
    }

    public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
    {
        List<SubjectCourseGroup> existingGroups = await _context.SubjectCourseGroups
            .Where(x => x.SubjectCourseId.Equals(request.SubjectCourseId))
            .Where(x => request.GroupIds.Contains(x.StudentGroupId))
            .ToListAsync(cancellationToken);

        IEnumerable<Guid> existingGroupIds = existingGroups.Select(x => x.StudentGroupId);
        IEnumerable<Guid> groupsToCreateIds = request.GroupIds.Except(existingGroupIds);

        var values = await _context.SubjectCourses
            .Include(x => x.Assignments)
            .ThenInclude(x => x.GroupAssignments)
            .Where(x => x.Id.Equals(request.SubjectCourseId))
            .Select(sc => new
            {
                Course = sc,
                Groups = _context.StudentGroups.Where(g => groupsToCreateIds.Contains(g.Id)).ToArray(),
            })
            .SingleOrDefaultAsync(cancellationToken);

        if (values is null)
            throw EntityNotFoundException.For<SubjectCourse>(request.SubjectCourseId);

        SubjectCourseGroup[] subjectCourseGroups = values.Groups
            .Select(x => values.Course.AddGroup(x))
            .ToArray();

        IEnumerable<(Assignment Assignment, StudentGroup Group)> groupAssignments = values.Course.Assignments
            .SelectMany(_ => values.Groups, (a, g) => (a, g));

        foreach ((Assignment assignment, StudentGroup group) in groupAssignments)
        {
            assignment.AddGroup(group, DateOnly.FromDateTime(DateTime.UnixEpoch));
        }

        _context.SubjectCourses.Update(values.Course);
        await _context.SaveChangesAsync(cancellationToken);

        SubjectCourseGroupDto[] groups = subjectCourseGroups.Select(x => x.ToDto()).ToArray();

        return new Response(groups);
    }
}