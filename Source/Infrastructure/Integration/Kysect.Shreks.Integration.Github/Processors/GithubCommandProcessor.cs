using Kysect.Shreks.Application.Commands.Commands;
using Kysect.Shreks.Application.Commands.Contexts;
using Kysect.Shreks.Application.Commands.Processors;
using Kysect.Shreks.Application.Commands.Result;

namespace Kysect.Shreks.Integration.Github.Processors;

public class GithubCommandProcessor : IShreksCommandProcessor<BaseShreksCommandResult>
{
    public async Task<BaseShreksCommandResult> Process(RateCommand rateCommand, ICommandContextCreator contextCreator)
    {
        try
        {
            var submissionId = await rateCommand.Execute(await contextCreator.CreateSubmissionContext());
            return new BaseShreksCommandResult(true, $"Created submission with id {submissionId}");
        }
        catch(Exception e) //TODO: catch different exceptions end write better messages
        {
            return new BaseShreksCommandResult(false, e.Message); 
        }
    }

    public async Task<BaseShreksCommandResult> Process(UpdateCommand updateCommand, ICommandContextCreator contextCreator)
    {
        try
        {
            var submissionDto = await updateCommand.Execute(await contextCreator.CreateBaseContext());
            return new BaseShreksCommandResult(true, string.Format($"Updated submission: {submissionDto}")); //md?
        }
        catch(Exception e) //TODO: catch different exceptions end write better messages
        {
            return new BaseShreksCommandResult(false, e.Message); 
        }
    }
}