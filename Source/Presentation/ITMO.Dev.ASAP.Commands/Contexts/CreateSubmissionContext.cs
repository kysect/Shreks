using ITMO.Dev.ASAP.Application.Dto.Study;
using MediatR;

namespace ITMO.Dev.ASAP.Commands.Contexts;

public class CreateSubmissionContext : BaseContext
{
    private readonly Func<CancellationToken, Task<SubmissionRateDto>> _createFunc;

    public CreateSubmissionContext(
        Guid issuerId,
        IMediator mediator,
        Func<CancellationToken, Task<SubmissionRateDto>> createFunc,
        string payload)
        : base(issuerId, mediator)
    {
        _createFunc = createFunc;
        Payload = payload;
    }

    public string Payload { get; }

    public Task<SubmissionRateDto> CreateAsync(CancellationToken cancellationToken)
    {
        return _createFunc.Invoke(cancellationToken);
    }
}