using CommandLine;
using Kysect.Shreks.Application.Commands.Commands;
using Kysect.Shreks.Common.Exceptions;

namespace Kysect.Shreks.Application.Commands.Parsers;

public class ShreksCommandParser : IShreksCommandParser
{
    private static readonly char[] ArgumentsSeparators = { ' ' };

    private readonly Type[] _commandTypes;

    public ShreksCommandParser()
    {
        _commandTypes = typeof(IShreksCommand).Assembly.GetTypes()
            .Where(type => type.IsAssignableTo(typeof(IShreksCommand)) && !type.IsInterface)
            .ToArray();
    }

    public IShreksCommand Parse(string command)
    {
        var result = Parser.Default.ParseArguments(GetCommandArguments(command), _commandTypes);
        
        if (result.Tag is ParserResultType.NotParsed)
            throw InvalidUserInputException.FailedToParseUserCommand();

        return (IShreksCommand)result.Value;
    }

    private static IEnumerable<string> GetCommandArguments(string command)
    {
        const StringSplitOptions splitOptions = StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries;
        return command.Split(ArgumentsSeparators, splitOptions);
    }
}