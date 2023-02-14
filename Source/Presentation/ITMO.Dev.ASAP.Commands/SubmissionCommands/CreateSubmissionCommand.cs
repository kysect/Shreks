using CommandLine;
using ITMO.Dev.ASAP.Application.Dto.Study;
using ITMO.Dev.ASAP.Commands.CommandVisitors;
using ITMO.Dev.ASAP.Commands.Contexts;
using Microsoft.Extensions.Logging;

namespace ITMO.Dev.ASAP.Commands.SubmissionCommands;

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