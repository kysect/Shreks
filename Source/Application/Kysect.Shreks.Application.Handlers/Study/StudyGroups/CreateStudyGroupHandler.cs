using Kysect.Shreks.Core.Study;
using Kysect.Shreks.DataAccess.Abstractions;
using Kysect.Shreks.Mapping.Mappings;
using MediatR;
using static Kysect.Shreks.Application.Contracts.Study.Commands.CreateStudyGroup;

namespace Kysect.Shreks.Application.Handlers.Study.StudyGroups;

internal class CreateStudyGroupHandler : IRequestHandler<Command, Response>
{
    private readonly IShreksDatabaseContext _context;

    public CreateStudyGroupHandler(IShreksDatabaseContext context)
    {
        _context = context;
    }

    public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
    {
        var studentGroup = new StudentGroup(Guid.NewGuid(), request.Name);

        _context.StudentGroups.Add(studentGroup);
        await _context.SaveChangesAsync(cancellationToken);

        return new Response(studentGroup.ToDto());
    }
}