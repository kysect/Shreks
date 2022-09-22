using CommandLine;
using Kysect.Shreks.Application.Commands.Contexts;
using Kysect.Shreks.Application.Dto.Study;
using Microsoft.Extensions.Logging;

namespace Kysect.Shreks.Application.Commands.Commands;

[Verb("/create-submission")]
public class CreateSubmissionCommand : IShreksCommand
{
    public async Task<SubmissionRateDto> ExecuteAsync(PayloadAndAssignmentContext context, ILogger logger, CancellationToken cancellationToken)
    {
        logger.LogInformation($"Handle /create-submission command for pr {context.Payload}");

        return await context.CommandSubmissionFactory.CreateSubmission(
            context.IssuerId,
            context.AssignmentId);
    }
}