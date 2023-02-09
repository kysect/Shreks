using Kysect.Shreks.Application.Contracts.Students.Notifications;
using Kysect.Shreks.Application.Dto.Users;
using Kysect.Shreks.Core.Study;
using Kysect.Shreks.Core.Users;
using Kysect.Shreks.DataAccess.Abstractions;
using Kysect.Shreks.DataAccess.Abstractions.Extensions;
using Kysect.Shreks.Mapping.Mappings;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static Kysect.Shreks.Application.Contracts.Students.Commands.TransferStudent;

namespace Kysect.Shreks.Application.Handlers.Students;

internal class TransferStudentHandler : IRequestHandler<Command, Response>
{
    private readonly IShreksDatabaseContext _context;
    private readonly IPublisher _publisher;

    public TransferStudentHandler(IShreksDatabaseContext context, IPublisher publisher)
    {
        _context = context;
        _publisher = publisher;
    }

    public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
    {
        Student student = await _context.Students.GetByIdAsync(request.StudentId, cancellationToken);

        StudentGroup group = await _context.StudentGroups
            .Include(x => x.Students)
            .GetByIdAsync(request.GroupId, cancellationToken);

        Guid? oldGroupId = student.Group?.Id;

        student.TransferToAnotherGroup(group);

        _context.Students.Update(student);
        _context.StudentGroups.Update(group);
        await _context.SaveChangesAsync(cancellationToken);

        StudentDto dto = student.ToDto();

        var notification = new StudentTransferred.Notification(dto.User.Id, request.GroupId, oldGroupId);
        await _publisher.PublishAsync(notification, cancellationToken);

        return new Response(dto);
    }
}