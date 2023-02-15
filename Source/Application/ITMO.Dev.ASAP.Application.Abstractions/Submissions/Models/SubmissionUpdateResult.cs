using ITMO.Dev.ASAP.Application.Dto.Study;

namespace ITMO.Dev.ASAP.Application.Abstractions.Submissions.Models;

public record SubmissionUpdateResult(SubmissionRateDto Submission, bool IsCreated);