using Kysect.Shreks.Commands.SubmissionCommands;

namespace Kysect.Shreks.Commands.Parsers;

public interface ISubmissionCommandParser
{
    ISubmissionCommand Parse(string command);
}