using CommandLine;
using Kysect.Shreks.Application.Dto.Study;
using Kysect.Shreks.Commands.CommandVisitors;
using Kysect.Shreks.Commands.Contexts;
using Microsoft.Extensions.Logging;

namespace Kysect.Shreks.Commands.SubmissionCommands;

[Verb("/create-submission")]
public class CreateSubmissionCommand : ISubmissionCommand<CreateSubmissionContext, SubmissionRateDto>
{
    public async Task<SubmissionRateDto> ExecuteAsync(
        CreateSubmissionContext context,
        ILogger logger,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Handle /create-submission command for pr {Payload}", context.Payload);

        return await context.CreateAsync(cancellationToken);
    }

    public Task AcceptAsync(ISubmissionCommandVisitor visitor)
    {
        return visitor.VisitAsync(this);
    }
}