namespace Kysect.Shreks.Application.Commands.Result;

public interface IShreksCommandResult
{
    public bool IsSuccess { get; }
    
    public string Message { get; }
}