using Kysect.Shreks.Application.Commands.Commands;
using Kysect.Shreks.Application.Commands.Contexts;
using Kysect.Shreks.Application.Commands.Processors;
using Kysect.Shreks.Application.Commands.Result;

namespace Kysect.Shreks.Integration.Github.Processors;

public class GithubCommandProcessor : IShreksCommandProcessor<BaseShreksCommandResult>
{
    public async Task<BaseShreksCommandResult> Process(RateCommand rateCommand, ICommandContextFactory contextFactory)
    {
        try
        {
            var submissionId = await rateCommand.Execute(await contextFactory.CreateSubmissionContext());
            return new BaseShreksCommandResult(true, $"Created submission with id {submissionId}");
        }
        catch(Exception e) //TODO: catch different exceptions and write better messages
        {
            return new BaseShreksCommandResult(false, e.Message); 
        }
    }

    public async Task<BaseShreksCommandResult> Process(UpdateCommand updateCommand, ICommandContextFactory contextFactory)
    {
        try
        {
            var submissionDto = await updateCommand.Execute(await contextFactory.CreateBaseContext());
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