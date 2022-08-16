using Kysect.Shreks.Application.Commands.Commands;
using Kysect.Shreks.Application.Commands.Contexts;
using Kysect.Shreks.Application.Commands.Processors;
using Kysect.Shreks.Application.Commands.Result;

namespace Kysect.Shreks.Integration.Github.Processors;

public class GithubCommandProcessor : IShreksCommandVisitor<BaseShreksCommandResult>
{
    private readonly ICommandContextFactory _contextFactory;
    private readonly CancellationToken _cancellationToken;

    public GithubCommandProcessor(ICommandContextFactory contextFactory, CancellationToken cancellationToken)
    {
        _contextFactory = contextFactory;
        _cancellationToken = cancellationToken;
    }

    public async Task<BaseShreksCommandResult> Visit(RateCommand rateCommand)
    {
        try
        {
            var context = await _contextFactory.CreateSubmissionContext();
            var submissionId = await rateCommand.ExecuteAsync(context);
            return new BaseShreksCommandResult(true, $"Created submission with id {submissionId}");
        }
        catch(Exception e) //TODO: catch different exceptions and write better messages
        {
            return new BaseShreksCommandResult(false, e.Message); 
        }
    }

    public async Task<BaseShreksCommandResult> Visit(UpdateCommand updateCommand)
    {
        try
        {
            var context = await _contextFactory.CreateBaseContext();
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