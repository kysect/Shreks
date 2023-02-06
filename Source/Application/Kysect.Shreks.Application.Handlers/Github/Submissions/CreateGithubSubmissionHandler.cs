using Kysect.Shreks.Application.Abstractions.Google;
using Kysect.Shreks.Application.Dto.Study;
using Kysect.Shreks.Application.Extensions;
using Kysect.Shreks.Application.Factories;
using Kysect.Shreks.Application.GithubWorkflow.Factories;
using Kysect.Shreks.Core.Submissions;
using Kysect.Shreks.DataAccess.Abstractions;
using MediatR;
using static Kysect.Shreks.Application.Contracts.Github.Commands.CreateGithubSubmission;

namespace Kysect.Shreks.Application.Handlers.Github.Submissions;

internal class CreateGithubSubmissionHandler : IRequestHandler<Command, Response>
{
    private readonly IShreksDatabaseContext _context;
    private readonly ITableUpdateQueue _tableUpdateQueue;

    public CreateGithubSubmissionHandler(IShreksDatabaseContext context, ITableUpdateQueue tableUpdateQueue)
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