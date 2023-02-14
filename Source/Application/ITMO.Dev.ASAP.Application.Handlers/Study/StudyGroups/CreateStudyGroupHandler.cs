using ITMO.Dev.ASAP.Application.Contracts.Study.StudyGroups.Notifications;
using ITMO.Dev.ASAP.Application.Dto.Study;
using ITMO.Dev.ASAP.Core.Study;
using ITMO.Dev.ASAP.DataAccess.Abstractions;
using ITMO.Dev.ASAP.Mapping.Mappings;
using MediatR;
using static ITMO.Dev.ASAP.Application.Contracts.Study.StudyGroups.Commands.CreateStudyGroup;

namespace ITMO.Dev.ASAP.Application.Handlers.Study.StudyGroups;

internal class CreateStudyGroupHandler : IRequestHandler<Command, Response>
{
    private readonly IDatabaseContext _context;
    private readonly IPublisher _publisher;

    public CreateStudyGroupHandler(IDatabaseContext context, IPublisher publisher)
    {
        _context = context;
        _publisher = publisher;
    }

    public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
    {
        var studentGroup = new StudentGroup(Guid.NewGuid(), request.Name);

        _context.StudentGroups.Add(studentGroup);
        await _context.SaveChangesAsync(cancellationToken);

        StudyGroupDto dto = studentGroup.ToDto();

        var notification = new StudyGroupCreated.Notification(dto);
        await _publisher.PublishAsync(notification, cancellationToken);

        return new Response(dto);
    }
}