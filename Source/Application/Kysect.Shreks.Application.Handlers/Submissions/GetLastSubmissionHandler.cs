using AutoMapper;
using Kysect.Shreks.Application.Dto.Study;
using Kysect.Shreks.Common.Exceptions;
using Kysect.Shreks.Core.Submissions;
using Kysect.Shreks.DataAccess.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static Kysect.Shreks.Application.Contracts.Submissions.Queries.GetLastSubmission;

namespace Kysect.Shreks.Application.Handlers.Submissions;

internal class GetLastSubmissionHandler : IRequestHandler<Query, Response>
{
    private readonly IShreksDatabaseContext _context;
    private readonly IMapper _mapper;

    public GetLastSubmissionHandler(IShreksDatabaseContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
    {
        Submission? submission = await _context.Submissions
            .Where(x => x.Student.UserId.Equals(request.StudentId))
            .Where(x => x.GroupAssignment.Assignment.Id.Equals(request.AssignmentId))
            .OrderByDescending(x => x.Code)
            .FirstOrDefaultAsync(cancellationToken);

        if (submission is null)
        {
            string message = $"Submission for student {request.StudentId} and assignment {request.AssignmentId} not found";
            throw new EntityNotFoundException(message);
        }

        SubmissionDto dto = _mapper.Map<SubmissionDto>(submission);

        return new Response(dto);
    }
}