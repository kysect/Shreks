using CommandLine;
using Kysect.Shreks.Application.Commands.Contexts;
using Kysect.Shreks.Core.Submissions;
using Microsoft.Extensions.Logging;

namespace Kysect.Shreks.Application.Commands.Commands;

[Verb("/activate")]
public class ActivateCommand : IShreksCommand
{
    public async Task<Submission> ExecuteAsync(SubmissionContext context, ILogger logger, CancellationToken cancellationToken)
    {
        logger.LogInformation($"Handle /activate command for submission {context.SubmissionId} from user {context.IssuerId}");

        return await context.SubmissionService.ActivateSubmissionAsync(
            context.SubmissionId,
            context.IssuerId,
            cancellationToken);
    }
}