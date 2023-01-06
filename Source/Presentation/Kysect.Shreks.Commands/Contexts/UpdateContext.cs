using Kysect.Shreks.Application.Dto.Study;
using Kysect.Shreks.Application.Dto.Users;
using Kysect.Shreks.Commands.Tools;
using MediatR;

namespace Kysect.Shreks.Commands.Contexts;

public class UpdateContext : BaseContext
{
    private readonly IDefaultSubmissionProvider _submissionProvider;

    public UpdateContext(
        Guid issuerId,
        IMediator mediator,
        AssignmentDto assignment,
        UserDto student,
        IDefaultSubmissionProvider submissionProvider)
        : base(issuerId, mediator)
    {
        Assignment = assignment;
        Student = student;
        _submissionProvider = submissionProvider;
    }

    public AssignmentDto Assignment { get; }

    public UserDto Student { get; }

    public Task<SubmissionDto> GetDefaultSubmissionAsync(CancellationToken cancellationToken)
    {
        return _submissionProvider.GetDefaultSubmissionAsync(Student.Id, Assignment.Id, cancellationToken);
    }
}