using ITMO.Dev.ASAP.Application.Dto.Study;

namespace ITMO.Dev.ASAP.Commands.Tools;

public interface IDefaultSubmissionProvider
{
    Task<SubmissionDto> GetDefaultSubmissionAsync(
        Guid studentId,
        Guid assignmentId,
        CancellationToken cancellationToken);
}