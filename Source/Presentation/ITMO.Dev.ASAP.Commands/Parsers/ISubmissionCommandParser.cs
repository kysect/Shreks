using ITMO.Dev.ASAP.Commands.SubmissionCommands;

namespace ITMO.Dev.ASAP.Commands.Parsers;

public interface ISubmissionCommandParser
{
    ISubmissionCommand Parse(string command);
}