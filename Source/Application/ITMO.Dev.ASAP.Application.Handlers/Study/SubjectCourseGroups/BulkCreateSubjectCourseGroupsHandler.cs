using ITMO.Dev.ASAP.Application.Contracts.Study.SubjectCourseGroups.Notifications;
using ITMO.Dev.ASAP.Application.Dto.SubjectCourses;
using ITMO.Dev.ASAP.Common.Exceptions;
using ITMO.Dev.ASAP.Core.Study;
using ITMO.Dev.ASAP.DataAccess.Abstractions;
using ITMO.Dev.ASAP.Mapping.Mappings;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static ITMO.Dev.ASAP.Application.Contracts.Study.SubjectCourseGroups.Commands.BulkCreateSubjectCourseGroups;

namespace ITMO.Dev.ASAP.Application.Handlers.Study.SubjectCourseGroups;

internal class BulkCreateSubjectCourseGroupsHandler : IRequestHandler<Command, Response>
{
    private readonly IDatabaseContext _context;
    private readonly IPublisher _publisher;

    public BulkCreateSubjectCourseGroupsHandler(IDatabaseContext context, IPublisher publisher)
    {
        _context = context;
        _publisher = publisher;
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

        _context.SubjectCourses.Update(values.Course);
        await _context.SaveChangesAsync(cancellationToken);

        SubjectCourseGroupDto[] groups = subjectCourseGroups.Select(x => x.ToDto()).ToArray();

        IEnumerable<SubjectCourseGroupCreated.Notification> notifications = groups
            .Select(g => new SubjectCourseGroupCreated.Notification(g));

        IEnumerable<Task> tasks = notifications.Select(x => _publisher.PublishAsync(x, cancellationToken));
        await Task.WhenAll(tasks);

        return new Response(groups);
    }
}