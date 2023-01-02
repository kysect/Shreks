using AutoMapper;
using Kysect.Shreks.Application.Dto.Study;
using Kysect.Shreks.Common.Exceptions;
using Kysect.Shreks.Core.Submissions;
using Kysect.Shreks.DataAccess.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static Kysect.Shreks.Application.Contracts.Submissions.Queries.GetSubmissionByCode;

namespace Kysect.Shreks.Application.Handlers.Submissions;

internal class GetSubmissionByCodeHandler : IRequestHandler<Query, Response>
{
    private readonly IShreksDatabaseContext _context;
    private readonly IMapper _mapper;

    public GetSubmissionByCodeHandler(IShreksDatabaseContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
    {
        Submission? submission = await _context.Submissions
            .Where(x => x.Student.UserId.Equals(request.StudentId))
            .Where(x => x.GroupAssignment.Assignment.Id.Equals(request.AssignmentId))
            .SingleOrDefaultAsync(cancellationToken);

        if (submission is null)
            throw new EntityNotFoundException($"Couldn't find submission with code ${request.Code}");

        SubmissionDto dto = _mapper.Map<SubmissionDto>(submission);

        return new Response(dto);
    }
}