using ITMO.Dev.ASAP.Core.Study;
using ITMO.Dev.ASAP.DataAccess.Abstractions;
using ITMO.Dev.ASAP.Mapping.Mappings;
using MediatR;
using static ITMO.Dev.ASAP.Application.Contracts.Study.Subjects.Commands.CreateSubject;

namespace ITMO.Dev.ASAP.Application.Handlers.Study.Subjects;

internal class CreateSubjectHandler : IRequestHandler<Command, Response>
{
    private readonly IDatabaseContext _context;

    public CreateSubjectHandler(IDatabaseContext context)
    {
        _context = context;
    }

    public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
    {
        var subject = new Subject(Guid.NewGuid(), request.Title);

        _context.Subjects.Add(subject);
        await _context.SaveChangesAsync(cancellationToken);

        return new Response(subject.ToDto());
    }
}