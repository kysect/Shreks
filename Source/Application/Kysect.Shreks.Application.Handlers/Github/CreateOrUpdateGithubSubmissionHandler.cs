using AutoMapper;
using Kysect.Shreks.Application.Abstractions.Google;
using Kysect.Shreks.Application.Dto.Study;
using Kysect.Shreks.Application.Factory;
using Kysect.Shreks.Application.Handlers.Extensions;
using Kysect.Shreks.Core.Extensions;
using Kysect.Shreks.Core.Models;
using Kysect.Shreks.Core.Specifications.Github;
using Kysect.Shreks.Core.Specifications.GroupAssignments;
using Kysect.Shreks.Core.Specifications.Submissions;
using Kysect.Shreks.Core.Submissions;
using Kysect.Shreks.Core.Users;
using Kysect.Shreks.Core.Tools;
using Kysect.Shreks.DataAccess.Abstractions;
using Kysect.Shreks.DataAccess.Abstractions.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;

using static Kysect.Shreks.Application.Abstractions.Github.Commands.CreateOrUpdateGithubSubmission;

namespace Kysect.Shreks.Application.Handlers.Github;

public class CreateOrUpdateGithubSubmissionHandler : IRequestHandler<Command, Response>
{
    private readonly ITableUpdateQueue _tableUpdateQueue;
    private readonly IShreksDatabaseContext _context;
    private readonly ISubmissionFactory _submissionFactory;
    private readonly IMapper _mapper;

    public CreateOrUpdateGithubSubmissionHandler(
        ITableUpdateQueue tableUpdateQueue,
        IShreksDatabaseContext context,
        IMapper mapper, ISubmissionFactory submissionFactory)
    {
        _tableUpdateQueue = tableUpdateQueue;
        _context = context;
        _mapper = mapper;
        _submissionFactory = submissionFactory;
    }

    public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
    {
        Guid userId = request.UserId;

        var submissionSpec = new FindLatestGithubSubmission(
            request.PullRequestDescriptor.Organization,
            request.PullRequestDescriptor.Repository,
            request.PullRequestDescriptor.PrNumber);

        var submission = await _context.SubmissionAssociations
            .WithSpecification(submissionSpec)
            .Where(x => x.State != SubmissionState.Completed)
            .FirstOrDefaultAsync(cancellationToken);

        bool isCreated = false;

        if (submission is null || submission.IsRated)
        {
            submission = await _submissionFactory.CreateGithubSubmissionAsync(
                request.UserId,
                request.AssignmentId,
                request.PullRequestDescriptor,
                cancellationToken);
            isCreated = true;
            _tableUpdateQueue.EnqueueSubmissionsQueueUpdate(submission.GetCourseId(), submission.GetGroupId());
        }
        else if (!await TriggeredByMentor(userId, request.PullRequestDescriptor.Organization))
        {
            submission.SubmissionDate = Calendar.CurrentDate;

            _context.Submissions.Update(submission);
            await _context.SaveChangesAsync(cancellationToken);
            _tableUpdateQueue.EnqueueSubmissionsQueueUpdate(submission.GetCourseId(), submission.GetGroupId());
        }

        _tableUpdateQueue.EnqueueSubmissionsQueueUpdate(submission.GetCourseId(), submission.GetGroupId());
        var dto = _mapper.Map<SubmissionDto>(submission);

        return new Response(isCreated, dto);
    }

    private async Task<Boolean> TriggeredByMentor(Guid userId, string organizationName)
    {
        Mentor? mentor = await _context.SubjectCourseAssociations
            .WithSpecification(new FindMentorByUsernameAndOrganization(userId, organizationName))
            .FirstOrDefaultAsync();

        return mentor is not null;
    }
}