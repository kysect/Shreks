using ITMO.Dev.ASAP.Application.Contracts.Study.StudyGroups.Notifications;
using ITMO.Dev.ASAP.Application.Dto.Study;
using ITMO.Dev.ASAP.Core.Study;
using ITMO.Dev.ASAP.DataAccess.Abstractions;
using ITMO.Dev.ASAP.DataAccess.Abstractions.Extensions;
using ITMO.Dev.ASAP.Mapping.Mappings;
using MediatR;
using static ITMO.Dev.ASAP.Application.Contracts.Study.StudyGroups.Commands.UpdateStudyGroup;

namespace ITMO.Dev.ASAP.Application.Handlers.Study.StudyGroups;

internal class UpdateStudyGroupHandler : IRequestHandler<Command, Response>
{
    private readonly IDatabaseContext _context;
    private readonly IPublisher _publisher;

    public UpdateStudyGroupHandler(IDatabaseContext context, IPublisher publisher)
    {
        _context = context;
        _publisher = publisher;
    }

    public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
    {
        StudentGroup studentGroup = await _context.StudentGroups.GetByIdAsync(request.Id, cancellationToken);
        studentGroup.Name = request.NewName;

        _context.StudentGroups.Update(studentGroup);
        await _context.SaveChangesAsync(cancellationToken);

        StudyGroupDto dto = studentGroup.ToDto();

        var notification = new StudyGroupUpdated.Notification(dto);
        await _publisher.PublishAsync(notification, cancellationToken);

        return new Response(dto);
    }
}