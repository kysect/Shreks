using Kysect.Shreks.Core.Study;

namespace Kysect.Shreks.Application.Abstractions.Google.Models;

public record struct StudentsQueue(IReadOnlyCollection<Submission> Submissions);