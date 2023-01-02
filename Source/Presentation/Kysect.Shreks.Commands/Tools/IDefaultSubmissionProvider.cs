using Kysect.Shreks.Application.Dto.Study;

namespace Kysect.Shreks.Commands.Tools;

public interface IDefaultSubmissionProvider
{
    Task<SubmissionDto> GetDefaultSubmissionAsync(Guid studentId, Guid assignmentId,
        CancellationToken cancellationToken);
}