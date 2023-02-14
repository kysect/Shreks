namespace ITMO.Dev.ASAP.Commands.Result;

public interface ICommandResult
{
    bool IsSuccess { get; }

    string Message { get; }
}