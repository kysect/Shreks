using Kysect.Shreks.Core.Study;
using Kysect.Shreks.DataAccess.Abstractions;
using Kysect.Shreks.DataAccess.Abstractions.Extensions;
using Kysect.Shreks.Mapping.Mappings;
using MediatR;
using static Kysect.Shreks.Application.Contracts.Study.Commands.UpdateSubject;

namespace Kysect.Shreks.Application.Handlers.Study.Subjects;

internal class UpdateSubjectHandler : IRequestHandler<Command, Response>
{
    private readonly IShreksDatabaseContext _context;

    public UpdateSubjectHandler(IShreksDatabaseContext context)
    {
        _context = context;
    }

    public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
    {
        Subject subject = await _context.Subjects.GetByIdAsync(request.Id, cancellationToken);
        subject.Title = request.NewName;

        _context.Subjects.Update(subject);
        await _context.SaveChangesAsync(cancellationToken);

        return new Response(subject.ToDto());
    }
}