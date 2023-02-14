using CommandLine;
using ITMO.Dev.ASAP.Application.Contracts.Study.Submissions.Commands;
using ITMO.Dev.ASAP.Application.Dto.Study;
using ITMO.Dev.ASAP.Commands.CommandVisitors;
using ITMO.Dev.ASAP.Commands.Contexts;
using Microsoft.Extensions.Logging;

namespace ITMO.Dev.ASAP.Commands.SubmissionCommands;

[Verb("/delete")]
public class DeleteCommand : ISubmissionCommand<SubmissionContext, SubmissionDto>
{
    public async Task<SubmissionDto> ExecuteAsync(
        SubmissionContext context,
        ILogger logger,
        CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Handle /delete command for submission {SubmissionId} from user {IssuerId}",
            context.SubmissionId,
            context.IssuerId);

        var command = new DeleteSubmission.Command(context.IssuerId, context.SubmissionId);
        DeleteSubmission.Response response = await context.Mediator.Send(command, cancellationToken);

        return response.Submission;
    }

    public Task AcceptAsync(ISubmissionCommandVisitor visitor)
    {
        return visitor.VisitAsync(this);
    }
}