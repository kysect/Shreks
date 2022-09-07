using Kysect.Shreks.Application.Dto.Github;
using Kysect.Shreks.Application.Factory;
using Kysect.Shreks.Application.Validators;
using Kysect.Shreks.Common.Exceptions;
using Kysect.Shreks.Core.Extensions;
using Kysect.Shreks.Core.Models;
using Kysect.Shreks.Core.Specifications.Submissions;
using Kysect.Shreks.Core.Submissions;
using Kysect.Shreks.Core.Tools;
using Kysect.Shreks.DataAccess.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Kysect.Shreks.Application.GithubWorkflow;

public class GithubSubmissionFactory
{
    private readonly IShreksDatabaseContext _context;
    //TODO: remove factory
    private readonly ISubmissionFactory _submissionFactory;

    public GithubSubmissionFactory(IShreksDatabaseContext context, ISubmissionFactory submissionFactory)
    {
        _context = context;
        _submissionFactory = submissionFactory;
    }

    public async Task<GithubSubmission> Handle(GithubPullRequestDescriptor pullRequestDescriptor, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(pullRequestDescriptor);

        var (sender,
        _,
        organization,
        repository,
        _,
            prNumber) = pullRequestDescriptor;

        // TODO: resolve user id from sender
        Guid userId = await GetUserId(pullRequestDescriptor);
        Guid assignmentId = await GetAssignmentId(pullRequestDescriptor);
        bool triggeredByMentor = await PermissionValidator.IsOrganizationMentor(_context, userId, organization);
        bool triggeredByAnotherUser = !PermissionValidator.IsRepositoryOwner(sender, repository);

        var submissionSpec = new FindLatestGithubSubmission(
            organization,
            repository,
            prNumber);

        var submission = await _context.SubmissionAssociations
            .WithSpecification(submissionSpec)
            .Where(x => x.State != SubmissionState.Completed)
            .FirstOrDefaultAsync(cancellationToken);

        bool isCreated = false;

        if (submission is null || submission.IsRated)
        {
            if (triggeredByAnotherUser && !triggeredByMentor)
            {
                var message = $"Repository {repository} is assigned to another student. " +
                              $"User {sender} does not have permission here. Close this PR and open new with correct account.";
                throw new DomainInvalidOperationException(message);
            }

            submission = await _submissionFactory.CreateGithubSubmissionAsync(
                userId,
                assignmentId,
                pullRequestDescriptor,
                cancellationToken);
            isCreated = true;

            _context.Submissions.Add(submission);
            await _context.SaveChangesAsync(cancellationToken);
        }
        else if (!triggeredByMentor)
        {
            submission.SubmissionDate = Calendar.CurrentDateTime;

            _context.Submissions.Update(submission);
            await _context.SaveChangesAsync(cancellationToken);

            if (triggeredByAnotherUser)
            {
                var message = $"Repository {repository} is assigned to another student. " +
                              $"Do not use {sender} account for this repository. Submission date will be updated.";
                throw new DomainInvalidOperationException(message);
            }
        }

        return submission;
    }

    public async Task<Guid> GetUserId(GithubPullRequestDescriptor pullRequestDescriptor)
    {
        throw new NotImplementedException();
    }

    public async Task<Guid> GetAssignmentId(GithubPullRequestDescriptor pullRequestDescriptor)
    {
        throw new NotImplementedException();
    }
}