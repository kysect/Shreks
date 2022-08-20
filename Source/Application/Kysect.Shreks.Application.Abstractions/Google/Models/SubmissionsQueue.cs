using Kysect.Shreks.Core.Study;

namespace Kysect.Shreks.Application.Abstractions.Google.Models;

public record struct SubmissionsQueue(IReadOnlyCollection<Submission> Submissions);