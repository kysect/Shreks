using AutoMapper;
using Kysect.Shreks.Application.Dto.Study;
using Kysect.Shreks.Application.Factory;
using Kysect.Shreks.Common.Exceptions;
using Kysect.Shreks.Core.Extensions;
using Kysect.Shreks.Core.Models;
using Kysect.Shreks.Core.Specifications.Submissions;
using Kysect.Shreks.DataAccess.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using static Kysect.Shreks.Application.Abstractions.Github.Commands.CreateGithubSubmission;

namespace Kysect.Shreks.Application.Handlers.Github;

public class CreateGithubSubmissionHandler : IRequestHandler<Command, Response>
{
    private readonly IShreksDatabaseContext _context;
    private readonly IMapper _mapper;
    private readonly ISubmissionFactory _submissionFactory;
    private readonly ILogger<CreateGithubSubmissionHandler> _logger;

    public CreateGithubSubmissionHandler(
        IShreksDatabaseContext context,
        ISubmissionFactory submissionFactory,
        IMapper mapper,
        ILogger<CreateGithubSubmissionHandler> logger)
    {
        _context = context;
        _submissionFactory = submissionFactory;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Create submission for {request.PullRequestDescriptor.ToString()}.");

        var submissionSpec = new FindLatestGithubSubmission(
            request.PullRequestDescriptor.Organization,
            request.PullRequestDescriptor.Repository,
            request.PullRequestDescriptor.PrNumber);

        var submission = await _context.SubmissionAssociations
            .WithSpecification(submissionSpec)
            .FirstOrDefaultAsync(cancellationToken);

        if (submission?.State is SubmissionState.Active or SubmissionState.Inactive)
            throw new SubmissionAlreadyExistsException(request.PullRequestDescriptor.PrNumber);

        submission = await _submissionFactory.CreateGithubSubmissionAsync(
            request.UserId,
            request.AssignmentId,
            request.PullRequestDescriptor,
            cancellationToken);

        await _context.Submissions.AddAsync(submission, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        var dto = _mapper.Map<SubmissionDto>(submission);

        return new Response(dto);
    }
}