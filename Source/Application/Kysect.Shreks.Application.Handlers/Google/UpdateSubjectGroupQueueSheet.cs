using Kysect.Shreks.Application.Abstractions.Google;
using Kysect.Shreks.Common.Exceptions;
using Kysect.Shreks.DataAccess.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static Kysect.Shreks.Application.Contracts.Google.Commands.UpdateSubjectGroupQueueSheet;

namespace Kysect.Shreks.Application.Handlers.Google;

public class UpdateSubjectGroupQueueSheet : IRequestHandler<Command>
{
    private readonly IShreksDatabaseContext _context;
    private readonly ITableUpdateQueue _tableUpdateQueue;

    public UpdateSubjectGroupQueueSheet(IShreksDatabaseContext context, ITableUpdateQueue tableUpdateQueue)
    {
        _context = context;
        _tableUpdateQueue = tableUpdateQueue;
    }

    public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
    {
        var subjectCourseGroup = await _context.SubjectCourseGroups
            .FirstOrDefaultAsync(g => g.StudentGroupId == request.StudentGroupId
                                      && g.SubjectCourseId == request.SubjectCourseId, cancellationToken);

        if (subjectCourseGroup is null)
        {
            var message =
                $"SubjectCourseGroup with StudentGroupId {request.StudentGroupId} and SubjectCourseId {request.SubjectCourseId} not found";
            throw new EntityNotFoundException(message);
        }

        _tableUpdateQueue.EnqueueSubmissionsQueueUpdate(request.SubjectCourseId, request.StudentGroupId);

        return Unit.Value;
    }
}