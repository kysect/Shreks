using Kysect.Shreks.Application.Commands.Commands;
using Kysect.Shreks.Application.Commands.Processors;
using Kysect.Shreks.Application.Commands.Result;
using MediatR;

namespace Kysect.Shreks.Integration.Github.Processors;

public class GithubCommandProcessor : IShreksCommandProcessor<BaseShreksCommandResult>
{
    private readonly IMediator _mediator;

    public GithubCommandProcessor(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<BaseShreksCommandResult> Process(RateCommand rateCommand, ShreksCommandContext context)
    {
        try
        {
            IEnumerable<IRequest> requests = rateCommand.GetRequest(context);
            foreach (var request in requests)
            {
                await _mediator.Send(request);
            }
            return new BaseShreksCommandResult(true, "");
        }
        catch(Exception e) //TODO: catch different exceptions end write better messages
        {
            return new BaseShreksCommandResult(false, e.Message); 
        }
    }

    public async Task<BaseShreksCommandResult> Process(UpdateCommand updateCommand, ShreksCommandContext context)
    {
        try
        {
            IEnumerable<IRequest> requests = updateCommand.GetRequest(context);
            //Submission? submission;
            foreach (var request in requests)
            {
                //write results to variable
                await _mediator.Send(request);
            }
            //TODO: add updated submission to message
            return new BaseShreksCommandResult(true, string.Format("Updated submission: ")); //md?
        }
        catch(Exception e) //TODO: catch different exceptions end write better messages
        {
            return new BaseShreksCommandResult(false, e.Message); 
        }
    }
}