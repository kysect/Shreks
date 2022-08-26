using AutoMapper;
using Kysect.Shreks.Application.Abstractions.Google;
using Kysect.Shreks.Application.Dto.Study;
using Kysect.Shreks.Application.Handlers.Extensions;
using Kysect.Shreks.Core.Extensions;
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
    private readonly IMapper _mapper;

    public CreateOrUpdateGithubSubmissionHandler(
        ITableUpdateQueue tableUpdateQueue,
        IShreksDatabaseContext context,
        IMapper mapper)
    {
        _tableUpdateQueue = tableUpdateQueue;
        _context = context;
        _mapper = mapper;
    }

    public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
    {
        var (sender
            _
            organization
            repository
            _
            prNumber) = request.PullRequestDescriptor;
        
        Guid userId = request.UserId;

        var submissionSpec = new FindLatestGithubSubmission(
            organization,
            repository,
            prNumber);

        var submission = await _context.SubmissionAssociations
            .WithSpecification(submissionSpec)
            .FirstOrDefaultAsync(cancellationToken);

        bool isCreated = false;

        if (submission is null || submission.IsRated)
        {
            submission = await CreateSubmission(request, cancellationToken);
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

    private async Task<GithubSubmission> CreateSubmission(Command request, CancellationToken cancellationToken)
    {
        var student = await _context.Students.GetByIdAsync(request.UserId, cancellationToken);
        var groupAssignmentSpec = new GetStudentGroupAssignment(student.Id, request.AssignmentId);

        var groupAssignment = await _context.GroupAssignments
            .WithSpecification(groupAssignmentSpec)
            .SingleAsync(cancellationToken);

        var studentAssignmentSubmissionsSpec = new GetStudentAssignmentSubmissions(
            student.Id, request.AssignmentId);

        var count = await _context.Submissions
            .WithSpecification(studentAssignmentSubmissionsSpec)
            .CountAsync(cancellationToken);

        var submission = new GithubSubmission
        (
            count + 1,
            student,
            groupAssignment,
            Calendar.CurrentDate,
            request.PullRequestDescriptor.Payload,
            request.PullRequestDescriptor.Organization,
            request.PullRequestDescriptor.Repository,
            request.PullRequestDescriptor.PrNumber
        );

        await _context.Submissions.AddAsync(submission, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return submission;
    }

    private async Task<Boolean> TriggeredByMentor(Guid userId, string organizationName)
    {
        Mentor? mentor = await _context.SubjectCourseAssociations
            .WithSpecification(new FindMentorByUsernameAndOrganization(userId, organizationName))
            .FirstOrDefaultAsync();

        return mentor is not null;
    }
}