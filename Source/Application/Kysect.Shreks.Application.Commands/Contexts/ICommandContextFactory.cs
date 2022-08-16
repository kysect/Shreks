namespace Kysect.Shreks.Application.Commands.Contexts;

public interface ICommandContextFactory
{
    Task<BaseContext> CreateBaseContext();
    Task<SubmissionContext> CreateSubmissionContext();
}