using CommandLine;

namespace Kysect.Shreks.Application.Commands.Commands;

public class ShreksCommandParser : IShreksCommandParser
{
    private readonly Type[] _commandTypes;

    public ShreksCommandParser()
    {
        _commandTypes = typeof(IShreksCommand).Assembly.GetTypes()
            .Where(type => type.IsAssignableTo(typeof(IShreksCommand)) && !type.IsInterface)
            .ToArray();
    }

    public IShreksCommand? Parse(string commandStr)
    {
        var result = Parser.Default.ParseArguments(commandStr.Split(), _commandTypes);
        if (result.Tag == ParserResultType.NotParsed)
        {
            //check if there was no command at all, then return null, else throw
        }

        return (IShreksCommand) result.Value;
    }
}