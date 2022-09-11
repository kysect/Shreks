using CommandLine;
using Kysect.Shreks.Application.Commands.Contexts;
using Kysect.Shreks.Application.Commands.Processors;
using Kysect.Shreks.Application.Commands.Result;
using Kysect.Shreks.Application.Dto.Study;
using Serilog;

namespace Kysect.Shreks.Application.Commands.Commands;

[Verb("/create-submission")]
public class CreateSubmissionCommand : IShreksCommand<PullRequestAndAssignmentContext, SubmissionRateDto>
{
    public async Task<SubmissionRateDto> ExecuteAsync(PullRequestAndAssignmentContext context, CancellationToken cancellationToken)
    {
        Log.Information($"Handle /create-submission command for pr {context.PullRequestDescriptor.Payload}");

        SubmissionRateDto result = await context.CommandSubmissionFactory.CreateSubmission(
            context.IssuerId,
            context.AssignmentId,
            context.PullRequestDescriptor);
        
        return result;
    }

    public Task<TResult> AcceptAsync<TResult>(IShreksCommandVisitor<TResult> visitor) where TResult : IShreksCommandResult
    {
        return visitor.VisitAsync(this);
    }
}