using Kysect.Shreks.Core.Study;
using Kysect.Shreks.Core.Submissions;
using Kysect.Shreks.Core.Users;

namespace Kysect.Shreks.Application.Commands.Contexts;

public class UpdateContext : BaseContext
{
    private readonly Func<Task<Submission>> _defaultSubmissionFactory;

    public ISubmissionService SubmissionService { get; set; }
    public Assignment Assignment { get; set; }
    public User Student { get; set; }

    public UpdateContext(
        Guid issuerId,
        User student,
        Assignment assignment,
        ISubmissionService submissionService,
        Func<Task<Submission>> defaultSubmissionFactory)
        : base(issuerId)
    {
        SubmissionService = submissionService;
        _defaultSubmissionFactory = defaultSubmissionFactory;
        Assignment = assignment;
        Student = student;
    }

    public Task<Submission> GetDefaultSubmissionAsync()
        => _defaultSubmissionFactory.Invoke();
}