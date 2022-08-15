namespace Kysect.Shreks.Application.Commands.Contexts;

public interface ICommandContextCreator
{
    Task<BaseContext> CreateBaseContext();
    Task<SubmissionContext> CreateSubmissionContext();
}