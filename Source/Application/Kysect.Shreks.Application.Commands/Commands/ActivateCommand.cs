using CommandLine;
using Kysect.Shreks.Application.Abstractions.Submissions.Commands;
using Kysect.Shreks.Application.Commands.Contexts;
using Kysect.Shreks.Application.Commands.Processors;
using Kysect.Shreks.Application.Commands.Result;
using Kysect.Shreks.Application.Dto.Study;
using Kysect.Shreks.Application.Dto.Users;

namespace Kysect.Shreks.Application.Commands.Commands;

[Verb("/activate")]
public class ActivateCommand : IShreksCommand<SubmissionContext, SubmissionDto>
{
    public async Task<SubmissionDto> ExecuteAsync(SubmissionContext context, CancellationToken cancellationToken)
    {
        var command = new UpdateSubmissionState.Command(
            context.IssuerId, context.Submission.Id, SubmissionStateDto.Active);

        var response = await context.Mediator.Send(command, cancellationToken);

        return response.Submission;
    }

    public Task<TResult> AcceptAsync<TResult>(IShreksCommandVisitor<TResult> visitor)
        where TResult : IShreksCommandResult
    {
        return visitor.VisitAsync(this);
    }
}