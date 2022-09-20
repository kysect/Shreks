using Kysect.Shreks.Application.Abstractions.Google;
using Kysect.Shreks.DataAccess.Abstractions;
using Kysect.Shreks.DataAccess.Abstractions.Extensions;
using MediatR;

using static Kysect.Shreks.Application.Abstractions.Google.Commands.UpdateSubjectCourseTable;

namespace Kysect.Shreks.Application.Handlers.Google;


public class UpdateSubjectCourseTableHandler : IRequestHandler<Command>
{
    private readonly IShreksDatabaseContext _context;
    private readonly ITableUpdateQueue _tableUpdateQueue;

    public UpdateSubjectCourseTableHandler(IShreksDatabaseContext context, ITableUpdateQueue tableUpdateQueue)
    {
        _context = context;
        _tableUpdateQueue = tableUpdateQueue;
    }

    public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
    {
        var subjectCourse = await _context.SubjectCourses.GetByIdAsync(request.SubjectCourseId, cancellationToken);

        _tableUpdateQueue.EnqueueCoursePointsUpdate(request.SubjectCourseId);
        foreach (var group in subjectCourse.Groups)
        {
            _tableUpdateQueue.EnqueueSubmissionsQueueUpdate(request.SubjectCourseId, group.StudentGroupId);
        }

        return Unit.Value;
    }
}