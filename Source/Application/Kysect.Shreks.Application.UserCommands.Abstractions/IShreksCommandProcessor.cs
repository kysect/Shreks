using Kysect.Shreks.Application.Dto.Study;
using Kysect.Shreks.Application.UserCommands.Abstractions.Commands;

namespace Kysect.Shreks.Application.UserCommands.Abstractions;

public interface IShreksCommandProcessor
{
    Task<SubmissionRateDto> Rate(RateCommand rateCommand, CancellationToken cancellationToken);
    Task<SubmissionRateDto> Update(UpdateCommand updateCommand, CancellationToken cancellationToken);
    Task<string> Help(HelpCommand helpCommand, CancellationToken token);
    Task<SubmissionDto> Activate(ActivateCommand activateCommand, CancellationToken cancellationToken);
    Task<SubmissionDto> Deactivate(DeactivateCommand deactivateCommand, CancellationToken cancellationToken);
    Task<SubmissionRateDto> CreateSubmission(CreateSubmissionCommand createSubmissionCommand, CancellationToken cancellationToken);
    Task<SubmissionDto> Delete(DeleteCommand deleteCommand, CancellationToken cancellationToken);
}