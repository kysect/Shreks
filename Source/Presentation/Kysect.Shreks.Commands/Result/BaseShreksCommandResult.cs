namespace Kysect.Shreks.Commands.Result;

public class BaseShreksCommandResult : IShreksCommandResult
{
    private BaseShreksCommandResult(bool isSuccess, string message)
    {
        IsSuccess = isSuccess;
        Message = message;
    }

    public bool IsSuccess { get; }
    public string Message { get; }

    public static BaseShreksCommandResult Success(string message)
    {
        return new BaseShreksCommandResult(true, message);
    }

    public static BaseShreksCommandResult Fail(string message)
    {
        return new BaseShreksCommandResult(false, message);
    }
}