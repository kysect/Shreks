using CommandLine;
using Kysect.Shreks.Application.Contracts.Submissions.Commands;
using Kysect.Shreks.Application.Dto.Study;
using Kysect.Shreks.Commands.CommandVisitors;
using Kysect.Shreks.Commands.Contexts;
using Microsoft.Extensions.Logging;

namespace Kysect.Shreks.Commands.SubmissionCommands;

[Verb("/mark-reviewed")]
public class MarkReviewedCommand : ISubmissionCommand<SubmissionContext, SubmissionDto>
{
    public async Task<SubmissionDto> ExecuteAsync(
        SubmissionContext context,
        ILogger logger,
        CancellationToken cancellationToken)
    {
        var command = new MarkSubmissionReviewed.Command(context.IssuerId, context.SubmissionId);
        MarkSubmissionReviewed.Response response = await context.Mediator.Send(command, cancellationToken);

        return response.Submission;
    }

    public Task AcceptAsync(ISubmissionCommandVisitor visitor)
    {
        return visitor.VisitAsync(this);
    }
}