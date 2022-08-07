using CommandLine;

namespace Kysect.Shreks.Application.Commands.Commands;

public class ShreksCommandParser
{
    private readonly Type[] _commandTypes;

    public ShreksCommandParser()
    {
        _commandTypes = typeof(ICommand).Assembly.GetTypes()
            .Where(type => type.IsAssignableTo(typeof(ICommand)) && !type.IsInterface)
            .ToArray();
    }

    public ICommand? Parse(string commandStr)
    {
        var result = Parser.Default.ParseArguments(commandStr.Split(), _commandTypes);
        if (result.Tag == ParserResultType.NotParsed)
        {
            //check if there was no command at all, then return null, else throw
        }

        return (ICommand) result.Value;
    }
}