using CommandLine;
using Kysect.Shreks.Application.Commands.Contexts;

namespace Kysect.Shreks.Application.Commands.Commands;

[Verb("/mark-reviewed")]
public class MarkReviewedCommand : IShreksCommand
{
    public Task ExecuteAsync(SubmissionContext context, CancellationToken cancellationToken)
    { 
        return context.SubmissionService.ReviewSubmissionAsync(
            context.SubmissionId,
            context.IssuerId,
            cancellationToken);
    }
}