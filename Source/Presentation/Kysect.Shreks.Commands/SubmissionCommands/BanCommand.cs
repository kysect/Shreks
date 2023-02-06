using CommandLine;
using Kysect.Shreks.Application.Contracts.Study.Submissions.Commands;
using Kysect.Shreks.Application.Contracts.Study.Submissions.Queries;
using Kysect.Shreks.Application.Dto.Study;
using Kysect.Shreks.Commands.CommandVisitors;
using Kysect.Shreks.Commands.Contexts;
using Microsoft.Extensions.Logging;

namespace Kysect.Shreks.Commands.SubmissionCommands;

[Verb("/ban")]
public class BanCommand : ISubmissionCommand<UpdateContext, SubmissionDto>
{
    public BanCommand(int? submissionCode)
    {
        SubmissionCode = submissionCode;
    }

    [Value(0, Required = false, MetaName = "SubmissionCode")]
    public int? SubmissionCode { get; }

    public async Task<SubmissionDto> ExecuteAsync(
        UpdateContext context,
        ILogger logger,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Handle /ban command for submission with code {SubmissionCode}", SubmissionCode);

        SubmissionDto submission = SubmissionCode is null
            ? await context.GetDefaultSubmissionAsync(cancellationToken)
            : await GetSubmissionByCodeAsync(context, SubmissionCode.Value, cancellationToken);

        var command = new BanSubmission.Command(context.IssuerId, submission.Id);
        BanSubmission.Response response = await context.Mediator.Send(command, cancellationToken);

        return response.Submission;
    }

    public Task AcceptAsync(ISubmissionCommandVisitor visitor)
    {
        return visitor.VisitAsync(this);
    }

    private async Task<SubmissionDto> GetSubmissionByCodeAsync(
        UpdateContext context,
        int code,
        CancellationToken cancellationToken)
    {
        var query = new GetSubmissionByCode.Query(context.Student.Id, context.Assignment.Id, code);
        GetSubmissionByCode.Response response = await context.Mediator.Send(query, cancellationToken);

        return response.Submission;
    }
}