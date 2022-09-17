using CommandLine;
using Kysect.Shreks.Application.Commands.Contexts;
using Kysect.Shreks.Core.Models;
using Kysect.Shreks.Core.Submissions;
using Microsoft.Extensions.Logging;

namespace Kysect.Shreks.Application.Commands.Commands;

[Verb("/delete")]
public class DeleteCommand : IShreksCommand
{
    public async Task<Submission> ExecuteAsync(SubmissionContext context, ILogger logger, CancellationToken cancellationToken)
    {
        logger.LogInformation($"Handle /delete command for submission {context.SubmissionId} from user {context.IssuerId}");

        return await context.SubmissionService.UpdateSubmissionState(
            context.SubmissionId,
            context.IssuerId,
            SubmissionState.Deleted,
            cancellationToken);
    }
}