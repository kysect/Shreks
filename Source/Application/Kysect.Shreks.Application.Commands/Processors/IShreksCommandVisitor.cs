using Kysect.Shreks.Application.Commands.Result;
using Kysect.Shreks.Application.UserCommands.Abstractions.Commands;

namespace Kysect.Shreks.Application.Commands.Processors;

public interface IShreksCommandVisitor<TResult> where TResult : IShreksCommandResult
{
    Task<TResult> VisitAsync(RateCommand rateCommand);
    Task<TResult> VisitAsync(UpdateCommand updateCommand);
    Task<TResult> VisitAsync(HelpCommand helpCommand);

    Task<TResult> VisitAsync(ActivateCommand command);
    Task<TResult> VisitAsync(DeactivateCommand command);
    Task<TResult> VisitAsync(CreateSubmissionCommand command);
    Task<TResult> VisitAsync(DeleteCommand command);
}

public static class ShreksCommandVisitorExtensions
{
    public static Task<TResult> Visit<TResult>(this IShreksCommandVisitor<TResult> visitor, IShreksCommand command) where TResult : IShreksCommandResult
    {
        return command switch
        {
            ActivateCommand activateCommand => visitor.Visit(activateCommand),
            CreateSubmissionCommand createSubmissionCommand => visitor.Visit(createSubmissionCommand),
            DeactivateCommand deactivateCommand => visitor.Visit(deactivateCommand),
            DeleteCommand deleteCommand => visitor.Visit(deleteCommand),
            HelpCommand helpCommand => visitor.VisitAsync(helpCommand),
            RateCommand rateCommand => visitor.VisitAsync(rateCommand),
            UpdateCommand updateCommand => visitor.Visit(updateCommand),
            _ => throw new ArgumentOutOfRangeException(nameof(command)),
        };
    }
}