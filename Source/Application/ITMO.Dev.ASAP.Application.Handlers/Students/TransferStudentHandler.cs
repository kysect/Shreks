using ITMO.Dev.ASAP.Application.Contracts.Students.Notifications;
using ITMO.Dev.ASAP.Application.Dto.Users;
using ITMO.Dev.ASAP.Core.Study;
using ITMO.Dev.ASAP.Core.Users;
using ITMO.Dev.ASAP.DataAccess.Abstractions;
using ITMO.Dev.ASAP.DataAccess.Abstractions.Extensions;
using ITMO.Dev.ASAP.Mapping.Mappings;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static ITMO.Dev.ASAP.Application.Contracts.Students.Commands.TransferStudent;

namespace ITMO.Dev.ASAP.Application.Handlers.Students;

internal class TransferStudentHandler : IRequestHandler<Command, Response>
{
    private readonly IDatabaseContext _context;
    private readonly IPublisher _publisher;

    public TransferStudentHandler(IDatabaseContext context, IPublisher publisher)
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