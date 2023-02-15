using CommandLine;
using ITMO.Dev.ASAP.Application.Contracts.Study.Submissions.Commands;
using ITMO.Dev.ASAP.Application.Contracts.Study.Submissions.Queries;
using ITMO.Dev.ASAP.Application.Dto.Study;
using ITMO.Dev.ASAP.Commands.CommandVisitors;
using ITMO.Dev.ASAP.Commands.Contexts;
using Microsoft.Extensions.Logging;

namespace ITMO.Dev.ASAP.Commands.SubmissionCommands;

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