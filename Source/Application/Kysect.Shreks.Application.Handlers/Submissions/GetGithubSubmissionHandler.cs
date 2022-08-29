using AutoMapper;
using Kysect.Shreks.Application.Dto.Study;
using Kysect.Shreks.Common.Exceptions;
using Kysect.Shreks.Core.Extensions;
using Kysect.Shreks.Core.Specifications.Submissions;
using Kysect.Shreks.DataAccess.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static Kysect.Shreks.Application.Abstractions.Submissions.Queries.GetGithubSubmission;

namespace Kysect.Shreks.Application.Handlers.Submissions;

public class GetGithubSubmissionHandler : IRequestHandler<Query, Response>
{
    private readonly IShreksDatabaseContext _context;
    private readonly IMapper _mapper;

    public GetGithubSubmissionHandler(IShreksDatabaseContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
    {
        var (organization, repository, prNumber) = request;
        var spec = new FindLatestGithubSubmission(organization, repository, prNumber);

        var submission = await _context.SubmissionAssociations
            .WithSpecification(spec)
            .FirstOrDefaultAsync(cancellationToken);
        
        if (submission is null)
            throw new EntityNotFoundException("No submission found");
        
        var submissionDto = _mapper.Map<SubmissionDto>(submission);
        return new Response(submissionDto);
    }
}