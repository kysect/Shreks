using AutoMapper;
using Kysect.Shreks.Application.Dto.Study;
using Kysect.Shreks.Core.Study;
using Kysect.Shreks.DataAccess.Abstractions;
using MediatR;
using static Kysect.Shreks.Application.Contracts.Study.Commands.CreateSubject;

namespace Kysect.Shreks.Application.Handlers.Study;

internal class CreateSubjectHandler : IRequestHandler<Command, Response>
{
    private readonly IShreksDatabaseContext _context;
    private readonly IMapper _mapper;

    public CreateSubjectHandler(IShreksDatabaseContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
    {
        var subject = new Subject(Guid.NewGuid(), request.Title);
        _context.Subjects.Add(subject);
        await _context.SaveChangesAsync(cancellationToken);
        return new Response(_mapper.Map<SubjectDto>(subject));
    }
}