using CommandLine;
using Kysect.Shreks.Application.Commands.Commands;

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
            throw new ArgumentException("Failed to parse command"); //TODO: handle different errors
        }

        return (IShreksCommand) result.Value;
    }
}