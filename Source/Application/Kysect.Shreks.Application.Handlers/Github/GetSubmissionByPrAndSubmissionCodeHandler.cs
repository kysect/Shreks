using AutoMapper;
using Kysect.Shreks.Application.Dto.Study;
using Kysect.Shreks.Core.SubmissionAssociations;
using Kysect.Shreks.DataAccess.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

using static Kysect.Shreks.Application.Abstractions.Github.Commands.GetSubmissionByPrAndSubmissionCode;

namespace Kysect.Shreks.Application.Handlers.Github;

public class GetSubmissionByPrAndSubmissionCodeHandler : IRequestHandler<Query, Response>
{
    private readonly IShreksDatabaseContext _context;
    private readonly IMapper _mapper;

    public GetSubmissionByPrAndSubmissionCodeHandler(IShreksDatabaseContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
    {
        GithubSubmissionAssociation submission = await _context
            .SubmissionAssociations
            .OfType<GithubSubmissionAssociation>()
            .Where(a => a.Organization == request.PullRequestDescriptor.Organization)
            .Where(a => a.Repository == request.PullRequestDescriptor.Repository)
            .Where(a => a.PrNumber == request.PullRequestDescriptor.PrNumber)
            .Where(a => a.Submission.Code == request.SubmissionCode)
            .FirstAsync(cancellationToken);

        var dto = _mapper.Map<SubmissionDto>(submission.Submission);

        return new Response(dto);
    }

}