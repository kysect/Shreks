using CommandLine;
using Kysect.Shreks.Application.Contracts.Submissions.Commands;
using Kysect.Shreks.Application.Dto.Study;
using Kysect.Shreks.Commands.CommandVisitors;
using Kysect.Shreks.Commands.Contexts;
using Microsoft.Extensions.Logging;

namespace Kysect.Shreks.Commands.SubmissionCommands;

[Verb("/delete")]
public class DeleteCommand : ISubmissionCommand<SubmissionContext, SubmissionDto>
{
    public async Task<SubmissionDto> ExecuteAsync(
        SubmissionContext context,
        ILogger logger,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Handle /delete command for submission {SubmissionId} from user {IssuerId}",
            context.SubmissionId, context.IssuerId);

        var command = new DeleteSubmission.Command(context.IssuerId, context.SubmissionId);
        DeleteSubmission.Response response = await context.Mediator.Send(command, cancellationToken);

        return response.Submission;
    }

    public Task AcceptAsync(ISubmissionCommandVisitor visitor)
    {
        return visitor.VisitAsync(this);
    }
}