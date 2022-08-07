using Kysect.Shreks.Application.Commands.Processors;
using Kysect.Shreks.Core.Users;

namespace Kysect.Shreks.Application.Commands.Commands;

public interface ICommand
{
    void Process(ICommandProcessor processor, User executor);
}