using Kysect.Shreks.Application.Contracts.Study.StudyGroups.Notifications;
using Kysect.Shreks.Application.Dto.Study;
using Kysect.Shreks.Core.Study;
using Kysect.Shreks.DataAccess.Abstractions;
using Kysect.Shreks.Mapping.Mappings;
using MediatR;
using static Kysect.Shreks.Application.Contracts.Study.StudyGroups.Commands.CreateStudyGroup;

namespace Kysect.Shreks.Application.Handlers.Study.StudyGroups;

internal class CreateStudyGroupHandler : IRequestHandler<Command, Response>
{
    private readonly IShreksDatabaseContext _context;
    private readonly IPublisher _publisher;

    public CreateStudyGroupHandler(IShreksDatabaseContext context, IPublisher publisher)
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