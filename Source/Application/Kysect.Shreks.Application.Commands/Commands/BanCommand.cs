using CommandLine;
using Kysect.Shreks.Application.Commands.Contexts;
using Kysect.Shreks.Core.Submissions;
using Microsoft.Extensions.Logging;

namespace Kysect.Shreks.Application.Commands.Commands;

[Verb("/ban")]
public class BanCommand : IShreksCommand
{
    public BanCommand(int submissionCode)
    {
        SubmissionCode = submissionCode;
    }

    [Value(0, Required = true, MetaName = "SubmissionCode")]
    public int SubmissionCode { get; }

    public async Task<Submission> ExecuteAsync(
        UpdateContext context,
        ILogger logger,
        CancellationToken cancellationToken)
    {
        logger.LogInformation($"Handle /ban command for submission with code {SubmissionCode}");

        Submission submission = await context.SubmissionService.GetSubmissionByCodeAsync(
            SubmissionCode,
            context.Student.Id,
            context.Assignment.Id,
            cancellationToken);

        return await context.SubmissionService.BanSubmissionAsync(
            submission.Id,
            context.IssuerId,
            cancellationToken);
    }
}