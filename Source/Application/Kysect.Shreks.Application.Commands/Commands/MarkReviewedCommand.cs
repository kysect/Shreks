using CommandLine;
using Kysect.Shreks.Application.Commands.Contexts;
using Kysect.Shreks.Core.Models;

namespace Kysect.Shreks.Application.Commands.Commands;

[Verb("/mark-reviewed")]
public class MarkReviewedCommand : IShreksCommand
{
    public Task ExecuteAsync(SubmissionContext context, CancellationToken cancellationToken)
    { 
        return context.SubmissionService.UpdateSubmissionState(
            context.SubmissionId,
            context.IssuerId,
            SubmissionState.Reviewed,
            cancellationToken);
    }
}