namespace ITMO.Dev.ASAP.Commands.Contexts;

public interface ISubmissionCommandContextFactory
{
    Task<BaseContext> BaseContextAsync(CancellationToken cancellationToken);

    Task<SubmissionContext> SubmissionContextAsync(CancellationToken cancellationToken);

    Task<UpdateContext> UpdateContextAsync(CancellationToken cancellationToken);

    Task<PayloadAndAssignmentContext> PayloadAndAssignmentContextAsync(CancellationToken cancellationToken);

    Task<CreateSubmissionContext> CreateSubmissionContextAsync(CancellationToken cancellationToken);
}