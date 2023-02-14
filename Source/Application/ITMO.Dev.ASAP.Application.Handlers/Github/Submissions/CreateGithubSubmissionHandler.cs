using ITMO.Dev.ASAP.Application.Abstractions.Google;
using ITMO.Dev.ASAP.Application.Dto.Study;
using ITMO.Dev.ASAP.Application.Extensions;
using ITMO.Dev.ASAP.Application.Factories;
using ITMO.Dev.ASAP.Application.GithubWorkflow.Factories;
using ITMO.Dev.ASAP.Core.Submissions;
using ITMO.Dev.ASAP.DataAccess.Abstractions;
using MediatR;
using static ITMO.Dev.ASAP.Application.Contracts.Github.Commands.CreateGithubSubmission;

namespace ITMO.Dev.ASAP.Application.Handlers.Github.Submissions;

internal class CreateGithubSubmissionHandler : IRequestHandler<Command, Response>
{
    private readonly IDatabaseContext _context;
    private readonly ITableUpdateQueue _tableUpdateQueue;

    public CreateGithubSubmissionHandler(IDatabaseContext context, ITableUpdateQueue tableUpdateQueue)
    {
        _context = context;
        _tableUpdateQueue = tableUpdateQueue;
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
            request.IssuerId,
            request.AssignmentId,
            cancellationToken);

        _context.Submissions.Add(submission);
        await _context.SaveChangesAsync(cancellationToken);

        _tableUpdateQueue.EnqueueSubmissionsQueueUpdate(submission.GetSubjectCourseId(), submission.GetGroupId());

        SubmissionRateDto dto = SubmissionRateDtoFactory.CreateFromSubmission(submission);

        return new Response(dto);
    }
}