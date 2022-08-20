using AutoMapper;
using Kysect.Shreks.Application.Dto.Study;
using Kysect.Shreks.Core.SubmissionAssociations;
using Kysect.Shreks.DataAccess.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Kysect.Shreks.Application.Handlers.Github;
using static Abstractions.Github.Queries.GetCurrentUnratedSubmissionByPrNumber;


public class GetCurrentUnratedSumbissionByPrNumberHandler : IRequestHandler<Query, Response>
{
    private readonly IShreksDatabaseContext _context;
    private readonly IMapper _mapper;


    public GetCurrentUnratedSumbissionByPrNumberHandler(IShreksDatabaseContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
    {
        var submission =  await _context.SubmissionAssociations
            .OfType<GithubPullRequestSubmissionAssociation>()
            .Where(a => 
                a.Organization == request.organisation 
                && a.Repository == request.repository
                && a.PullRequestNumber == request.PrNumber
                && a.Submission.Points.Value != 0.0) //TODO: make points nullable, as 0 is valid rating
            .OrderByDescending(a => a.Submission.SubmissionDate)
            .Select(a => a.Submission)
            .FirstOrDefaultAsync(cancellationToken);

        return new Response(_mapper.Map<SubmissionDto>(submission));
    }
}