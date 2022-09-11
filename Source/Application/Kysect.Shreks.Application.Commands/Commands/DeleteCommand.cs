using CommandLine;
using Kysect.Shreks.Application.Commands.Contexts;
using Kysect.Shreks.Core.Models;
using Kysect.Shreks.Core.Submissions;
using Microsoft.Extensions.Logging;

namespace Kysect.Shreks.Application.Commands.Commands;

[Verb("/delete")]
public class DeleteCommand : IShreksCommand
{
    public async Task<Submission> Execute(SubmissionContext context, ILogger logger, CancellationToken cancellationToken)
    {
        logger.LogInformation($"Handle /delete command for submission {context.Submission.Id} from user {context.IssuerId}");

        return await context.SubmissionService.UpdateSubmissionState(
            context.Submission.Id,
            context.IssuerId,
            SubmissionState.Deleted,
            cancellationToken);
    }
}