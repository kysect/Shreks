using CommandLine;

namespace Kysect.Shreks.Integration.Github.Commands;

public class CommandParser
{
    private readonly Type[] _commandTypes;

    public CommandParser()
    {
        _commandTypes = (from type in typeof(ICommand).Assembly.GetTypes()
            where type.IsAssignableTo(typeof(ICommand)) && !type.IsInterface
            select type).ToArray();
    }

    public ICommand? Parse(String commandStr)
    {
        var result = CommandLine.Parser.Default.ParseArguments(commandStr.Split(), _commandTypes);
        if (result.Tag == ParserResultType.NotParsed)
        {
            //check if there was no command at all, then return null, else throw
        }

        return result.Value as ICommand;
    }
}