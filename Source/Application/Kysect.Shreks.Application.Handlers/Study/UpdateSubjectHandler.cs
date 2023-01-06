using AutoMapper;
using Kysect.Shreks.Application.Dto.Study;
using Kysect.Shreks.Core.Study;
using Kysect.Shreks.DataAccess.Abstractions;
using Kysect.Shreks.DataAccess.Abstractions.Extensions;
using MediatR;
using static Kysect.Shreks.Application.Contracts.Study.Commands.UpdateSubject;

namespace Kysect.Shreks.Application.Handlers.Study;

internal class UpdateSubjectHandler : IRequestHandler<Command, Response>
{
    private readonly IShreksDatabaseContext _context;
    private readonly IMapper _mapper;

    public UpdateSubjectHandler(IShreksDatabaseContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
    {
        Subject subject = await _context.Subjects.GetByIdAsync(request.Id, cancellationToken);
        subject.Title = request.NewName;
        await _context.SaveChangesAsync(cancellationToken);
        return new Response(_mapper.Map<SubjectDto>(subject));
    }
}