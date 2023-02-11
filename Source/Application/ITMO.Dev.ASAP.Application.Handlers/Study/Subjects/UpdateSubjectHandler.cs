using ITMO.Dev.ASAP.Core.Study;
using ITMO.Dev.ASAP.DataAccess.Abstractions;
using ITMO.Dev.ASAP.DataAccess.Abstractions.Extensions;
using ITMO.Dev.ASAP.Mapping.Mappings;
using MediatR;
using static ITMO.Dev.ASAP.Application.Contracts.Study.Subjects.Commands.UpdateSubject;

namespace ITMO.Dev.ASAP.Application.Handlers.Study.Subjects;

internal class UpdateSubjectHandler : IRequestHandler<Command, Response>
{
    private readonly IDatabaseContext _context;

    public UpdateSubjectHandler(IDatabaseContext context)
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