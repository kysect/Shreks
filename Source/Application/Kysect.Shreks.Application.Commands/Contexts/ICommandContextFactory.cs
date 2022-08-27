namespace Kysect.Shreks.Application.Commands.Contexts;

public interface ICommandContextFactory
{
    Task<BaseContext> CreateBaseContext(CancellationToken cancellationToken);
    Task<SubmissionContext> CreateSubmissionContext(CancellationToken cancellationToken);
    Task<PullRequestContext> CreatePullRequestContext(CancellationToken cancellationToken);
}