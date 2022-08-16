using Kysect.Shreks.Application.Commands.Commands;
using Kysect.Shreks.Application.Commands.Contexts;
using Kysect.Shreks.Application.Commands.Processors;
using Kysect.Shreks.Application.Commands.Result;

namespace Kysect.Shreks.Integration.Github.Processors;

public class GithubCommandProcessor : IShreksCommandVisitor<BaseShreksCommandResult>
{
    public async Task<BaseShreksCommandResult> Visit(RateCommand rateCommand, ICommandContextFactory contextFactory,
        CancellationToken cancellationToken)
    {
        try
        {
            var submissionId = await rateCommand.ExecuteAsync(await contextFactory.CreateSubmissionContext(cancellationToken));
            return new BaseShreksCommandResult(true, $"Created submission with id {submissionId}");
        }
        catch(Exception e) //TODO: catch different exceptions and write better messages
        {
            return new BaseShreksCommandResult(false, e.Message); 
        }
    }

    public async Task<BaseShreksCommandResult> Visit(UpdateCommand updateCommand, ICommandContextFactory contextFactory,
        CancellationToken cancellationToken)
    {
        try
        {
            var context = await contextFactory.CreateBaseContext(cancellationToken);
            var submissionDto = await updateCommand.ExecuteAsync(context);
            return new BaseShreksCommandResult(true, 
                $"Updated submission - " +
                $"points: {submissionDto.Points}, " +
                $"Extra points: {submissionDto.ExtraPoints}, " +
                $"Date: {submissionDto.SubmissionDateTime}"); //md?
        }
        catch(Exception e) //TODO: catch different exceptions and write better messages
        {
            return new BaseShreksCommandResult(false, e.Message); 
        }
    }
}