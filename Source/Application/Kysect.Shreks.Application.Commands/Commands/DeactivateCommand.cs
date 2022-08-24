using CommandLine;
using Kysect.Shreks.Application.Abstractions.Submissions.Commands;
using Kysect.Shreks.Application.Commands.Contexts;
using Kysect.Shreks.Application.Commands.Processors;
using Kysect.Shreks.Application.Commands.Result;
using Kysect.Shreks.Application.Dto.Study;

namespace Kysect.Shreks.Application.Commands.Commands;

[Verb("/deactivate")]
public class DeactivateCommand : IShreksCommand<SubmissionContext, SubmissionDto>
{
    public async Task<SubmissionDto> ExecuteAsync(SubmissionContext context, CancellationToken cancellationToken)
    {
        var command = new DeactivateSubmission.Command(context.IssuerId, context.Submission.Id);
        var response = await context.Mediator.Send(command, cancellationToken);

        return response.Submission;
    }

    public Task<TResult> AcceptAsync<TResult>(IShreksCommandVisitor<TResult> visitor) where TResult : IShreksCommandResult
        => visitor.VisitAsync(this);
}