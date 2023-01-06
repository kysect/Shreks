using Kysect.Shreks.Application.Dto.Study;

namespace Kysect.Shreks.Application.Abstractions.Submissions.Models;

public record SubmissionUpdateResult(SubmissionRateDto Submission, bool IsCreated);