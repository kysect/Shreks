namespace Kysect.Shreks.Application.Commands.Result;

public class BaseShreksCommandResult : IShreksCommandResult
{
    public BaseShreksCommandResult(bool isSuccess, string message)
    {
        IsSuccess = isSuccess;
        Message = message;
    }

    public bool IsSuccess { get; }
    public string Message { get; }
}