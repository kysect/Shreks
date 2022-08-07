using System.Reflection.Metadata;
using Kysect.Shreks.Application.Commands.Commands;
using Kysect.Shreks.Core.Users;

namespace Kysect.Shreks.Application.Commands.Processors;

public interface ICommandProcessor
{
    void Process(RateCommand rateCommand, User executor); //help with name please
    void Process(UpdateCommand updateCommand, User executor); //help with name please
}