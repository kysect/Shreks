namespace ITMO.Dev.ASAP.Commands.Result;

public class BaseCommandResult : ICommandResult
{
    private BaseCommandResult(bool isSuccess, string message)
    {
        IsSuccess = isSuccess;
        Message = message;
    }

    public bool IsSuccess { get; }

    public string Message { get; }

    public static BaseCommandResult Success(string message)
    {
        return new BaseCommandResult(true, message);
    }

    public static BaseCommandResult Fail(string message)
    {
        return new BaseCommandResult(false, message);
    }
}