namespace Kysect.Shreks.Application.Commands.Result;

public class BaseShreksCommandResult : IShreksCommandResult
{
    public bool IsSuccess { get; }
    public string Message { get; }

    private BaseShreksCommandResult(bool isSuccess, string message)
    {
        IsSuccess = isSuccess;
        Message = message;
    }

    public static BaseShreksCommandResult Success(string message)
    {
        return new BaseShreksCommandResult(isSuccess: true, message);
    }

    public static BaseShreksCommandResult Fail(string message)
    {
        return new BaseShreksCommandResult(isSuccess: false, message);
    }
}