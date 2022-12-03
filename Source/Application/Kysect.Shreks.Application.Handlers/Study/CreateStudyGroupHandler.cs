using AutoMapper;
using Kysect.Shreks.Application.Dto.Study;
using Kysect.Shreks.Core.Study;
using Kysect.Shreks.DataAccess.Abstractions;
using MediatR;
using static Kysect.Shreks.Application.Contracts.Study.Commands.CreateStudyGroup;

namespace Kysect.Shreks.Application.Handlers.Study;

internal class CreateStudyGroupHandler : IRequestHandler<Command, Response>
{
    private readonly IShreksDatabaseContext _context;
    private readonly IMapper _mapper;

    public CreateStudyGroupHandler(IShreksDatabaseContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
    {
        var studentGroup = new StudentGroup(request.Name);
        _context.StudentGroups.Add(studentGroup);
        await _context.SaveChangesAsync(cancellationToken);
        return new Response(_mapper.Map<StudyGroupDto>(studentGroup));
    }
}