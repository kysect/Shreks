using CommandLine;
using Kysect.Shreks.Application.Commands.Commands;
using Kysect.Shreks.Common.Exceptions;

namespace Kysect.Shreks.Application.Commands.Parsers;

public class ShreksCommandParser : IShreksCommandParser
{
    private readonly Type[] _commandTypes;

    public ShreksCommandParser()
    {
        _commandTypes = typeof(IShreksCommand).Assembly.GetTypes()
            .Where(type => type.IsAssignableTo(typeof(IShreksCommand)) && !type.IsInterface)
            .ToArray();
    }

    public IShreksCommand Parse(string commandStr)
    {
        var result = Parser.Default.ParseArguments(commandStr.Split(), _commandTypes);
        if (result.Tag == ParserResultType.NotParsed)
        {
            throw new InvalidUserInputException("Failed to parse user command. Ensure that all arguments is correct or call /help.");
        }

        return (IShreksCommand) result.Value;
    }
}