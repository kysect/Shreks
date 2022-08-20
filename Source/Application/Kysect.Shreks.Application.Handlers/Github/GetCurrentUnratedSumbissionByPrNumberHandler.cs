using AutoMapper;
using Kysect.Shreks.Application.Abstractions.Exceptions;
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
                a.Organization == request.Organisation 
                && a.Repository == request.Repository
                && a.PullRequestNumber == request.PrNumber
                && a.Submission.Rating == null)
            .OrderByDescending(a => a.Submission.SubmissionDate)
            .Select(a => a.Submission)
            .FirstOrDefaultAsync(cancellationToken);

        if (submission is null)
            throw new EntityNotFoundException($"No unrated submission in pr " +
                                              $"{request.Organisation}/${request.Repository}/${request.PrNumber}");
        
        return new Response(_mapper.Map<SubmissionDto>(submission));
    }
}