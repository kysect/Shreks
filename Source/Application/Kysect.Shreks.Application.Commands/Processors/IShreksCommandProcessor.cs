using Kysect.Shreks.Application.Commands.Commands;
using Kysect.Shreks.Application.Dto.Study;
using Kysect.Shreks.Core.Submissions;

namespace Kysect.Shreks.Application.Commands.Processors;

public interface IShreksCommandProcessor
{
    Task<SubmissionRateDto> Rate(RateCommand rateCommand, CancellationToken cancellationToken);
    Task<SubmissionRateDto> Update(UpdateCommand updateCommand, CancellationToken cancellationToken);
    Task<string> Help(HelpCommand helpCommand, CancellationToken token);
    Task<Submission> Activate(ActivateCommand activateCommand, CancellationToken cancellationToken);
    Task<Submission> Deactivate(DeactivateCommand deactivateCommand, CancellationToken cancellationToken);
    Task<SubmissionRateDto> CreateSubmission(CreateSubmissionCommand createSubmissionCommand, CancellationToken cancellationToken);
    Task<Submission> Delete(DeleteCommand deleteCommand, CancellationToken cancellationToken);
}