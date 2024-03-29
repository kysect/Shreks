using AutoMapper;
using Kysect.Shreks.Application.Dto.Study;
using Kysect.Shreks.Common.Exceptions;
using Kysect.Shreks.Core.SubmissionAssociations;
using Kysect.Shreks.Core.Submissions;
using Kysect.Shreks.DataAccess.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static Kysect.Shreks.Application.Contracts.Github.Queries.GetLastPullRequestSubmission;

namespace Kysect.Shreks.Application.Handlers.Github;

internal class GetLastPullRequestSubmissionHandler : IRequestHandler<Query, Response>
{
    private readonly IShreksDatabaseContext _context;
    private readonly IMapper _mapper;

    public GetLastPullRequestSubmissionHandler(IShreksDatabaseContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
    {
        Submission? submission = await _context.Submissions
            .Where(x => x.Student.UserId.Equals(request.UserId))
            .Where(x => x.GroupAssignment.Assignment.Id.Equals(request.AssignmentId))
            .SelectMany(x => x.Associations)
            .OfType<GithubSubmissionAssociation>()
            .Where(x => x.PrNumber.Equals(request.PullRequestNumber))
            .Select(x => x.Submission)
            .OrderByDescending(x => x.Code)
            .FirstOrDefaultAsync(cancellationToken);

        if (submission is null)
            throw new EntityNotFoundException("Submission not found");

        SubmissionDto dto = _mapper.Map<SubmissionDto>(submission);

        return new Response(dto);
    }
}