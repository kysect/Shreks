using Kysect.Shreks.Application.UserCommands.Abstractions.Commands;

namespace Kysect.Shreks.Application.Commands.Parsers;

public interface IShreksCommandParser
{
    IShreksCommand Parse(string commandStr);
}