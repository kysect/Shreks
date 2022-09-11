using Kysect.Shreks.Core.Queue.Building;
using Kysect.Shreks.Core.Study;
using Kysect.Shreks.DataAccess.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static Kysect.Shreks.Application.Abstractions.SubjectCourses.Commands.UpdateDefaultSubjectCourseQueue;

namespace Kysect.Shreks.Application.Handlers.SubjectCourses;

internal class UpdateDefaultSubjectCourseQueueHandler : IRequestHandler<Command>
{
    private readonly IShreksDatabaseContext _context;

    public UpdateDefaultSubjectCourseQueueHandler(IShreksDatabaseContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
    {
        List<SubjectCourseGroup> groups = await _context.SubjectCourseGroups
            .Where(x => x.SubjectCourseId.Equals(request.SubjectCourseId))
            .ToListAsync(cancellationToken);

        foreach (var group in groups)
        {
            var builder = new DefaultQueueBuilder(group.StudentGroup, request.SubjectCourseId);
            group.Queue = builder.Build();
            _context.SubjectCourseGroups.Update(group);
        }

        await _context.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}