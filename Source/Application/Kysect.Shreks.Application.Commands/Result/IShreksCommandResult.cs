namespace Kysect.Shreks.Application.Commands.Result;

public interface IShreksCommandResult
{
    bool IsSuccess { get; }
    
    string Message { get; }
}