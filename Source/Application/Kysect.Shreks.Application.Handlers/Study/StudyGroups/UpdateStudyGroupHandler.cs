using Kysect.Shreks.Core.Study;
using Kysect.Shreks.DataAccess.Abstractions;
using Kysect.Shreks.DataAccess.Abstractions.Extensions;
using Kysect.Shreks.Mapping.Mappings;
using MediatR;
using static Kysect.Shreks.Application.Contracts.Study.StudyGroups.Commands.UpdateStudyGroup;

namespace Kysect.Shreks.Application.Handlers.Study.StudyGroups;

internal class UpdateStudyGroupHandler : IRequestHandler<Command, Response>
{
    private readonly IShreksDatabaseContext _context;

    public UpdateStudyGroupHandler(IShreksDatabaseContext context)
    {
        _context = context;
    }

    public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
    {
        StudentGroup studentGroup = await _context.StudentGroups.GetByIdAsync(request.Id, cancellationToken);
        studentGroup.Name = request.NewName;

        _context.StudentGroups.Update(studentGroup);
        await _context.SaveChangesAsync(cancellationToken);

        return new Response(studentGroup.ToDto());
    }
}