using Kysect.Shreks.Application.Commands.Commands;

namespace Kysect.Shreks.Application.Commands.Parsers;

public interface IShreksCommandParser
{
    IShreksCommand Parse(string commandStr);
}