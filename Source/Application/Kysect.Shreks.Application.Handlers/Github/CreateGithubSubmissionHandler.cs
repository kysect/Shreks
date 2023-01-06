using Kysect.Shreks.Application.Dto.Study;
using Kysect.Shreks.Application.Factories;
using Kysect.Shreks.Application.GithubWorkflow.Factories;
using Kysect.Shreks.Core.Submissions;
using Kysect.Shreks.DataAccess.Abstractions;
using MediatR;
using static Kysect.Shreks.Application.Contracts.Github.Commands.CreateGithubSubmission;

namespace Kysect.Shreks.Application.Handlers.Github;

internal class CreateGithubSubmissionHandler : IRequestHandler<Command, Response>
{
    private readonly IShreksDatabaseContext _context;

    public CreateGithubSubmissionHandler(IShreksDatabaseContext context)
    {
        _context = context;
    }

    public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
    {
        var factory = new GithubSubmissionFactory(
            _context,
            request.OrganizationName,
            request.RepositoryName,
            request.PullRequestNumber,
            request.Payload);

        Submission submission = await factory.CreateAsync(
            request.IssuerId, request.AssignmentId, cancellationToken);

        _context.Submissions.Add(submission);
        await _context.SaveChangesAsync(cancellationToken);

        SubmissionRateDto dto = SubmissionRateDtoFactory.CreateFromSubmission(submission);

        return new Response(dto);
    }
}