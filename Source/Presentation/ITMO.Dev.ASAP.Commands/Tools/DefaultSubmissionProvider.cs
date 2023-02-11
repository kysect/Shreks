using ITMO.Dev.ASAP.Application.Contracts.Study.Submissions.Queries;
using ITMO.Dev.ASAP.Application.Dto.Study;
using MediatR;

namespace ITMO.Dev.ASAP.Commands.Tools;

public class DefaultSubmissionProvider : IDefaultSubmissionProvider
{
    private readonly IMediator _mediator;

    public DefaultSubmissionProvider(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<SubmissionDto> GetDefaultSubmissionAsync(
        Guid studentId,
        Guid assignmentId,
        CancellationToken cancellationToken)
    {
        var query = new GetLastSubmission.Query(studentId, assignmentId);
        GetLastSubmission.Response response = await _mediator.Send(query, cancellationToken);

        return response.Submission;
    }
}