using CommandLine;
using Kysect.Shreks.Application.Commands.Contexts;
using Microsoft.Extensions.Logging;

namespace Kysect.Shreks.Application.Commands.Commands;

[Verb("/compromise")]
public class CompromiseSubmissionCommand : IShreksCommand
{
    public Task ExecuteAsync(SubmissionContext context, ILogger logger, CancellationToken cancellationToken)
    {
        logger.LogInformation($"Compromised submission {context.SubmissionId} by user {context.IssuerId}");

        return context.SubmissionService.CompromiseSubmissionAsync(
            context.SubmissionId,
            context.IssuerId,
            cancellationToken);
    }
}