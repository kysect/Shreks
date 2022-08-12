namespace Kysect.Shreks.Application.Commands.Commands;

public interface IShreksCommandParser
{
    IShreksCommand? Parse(string commandStr);
}