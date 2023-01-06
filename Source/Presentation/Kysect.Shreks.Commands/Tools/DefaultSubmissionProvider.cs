using Kysect.Shreks.Application.Contracts.Submissions.Queries;
using Kysect.Shreks.Application.Dto.Study;
using MediatR;

namespace Kysect.Shreks.Commands.Tools;

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