namespace Kysect.Shreks.Commands.Result;

public interface IShreksCommandResult
{
    bool IsSuccess { get; }

    string Message { get; }
}