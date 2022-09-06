using AutoMapper;
using Kysect.Shreks.Application.Dto.Study;
using Kysect.Shreks.Common.Exceptions;
using Kysect.Shreks.Core.SubmissionAssociations;
using Kysect.Shreks.DataAccess.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static Kysect.Shreks.Application.Abstractions.Github.Queries.GetLastSubmissionByPr;

namespace Kysect.Shreks.Application.Handlers.Github;

public class GetLastSubmissionByPrHandler : IRequestHandler<Query, Response>
{
    private readonly IShreksDatabaseContext _context;
    private readonly IMapper _mapper;

    public GetLastSubmissionByPrHandler(IShreksDatabaseContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
    {
        var submission = _context.SubmissionAssociations
            .OfType<GithubSubmissionAssociation>()
            .Where(a =>
                a.Organization == request.PullRequestDescriptor.Organization
                && a.Repository == request.PullRequestDescriptor.Repository
                && a.PrNumber == request.PullRequestDescriptor.PrNumber)
            .Select(s => s.Submission)
            .OrderByDescending(s => s.SubmissionDate)
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);

        if (submission is null)
        {
            var message = $"No submission in pr {request.PullRequestDescriptor.Payload}";
            throw new EntityNotFoundException(message);
        }

        var dto = _mapper.Map<SubmissionDto>(submission);

        return new Response(dto);
    }
}