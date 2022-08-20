using Kysect.Shreks.Application.Dto.Study;
using Kysect.Shreks.Application.Dto.Users;

namespace Kysect.Shreks.Application.Dto.Tables;

public record QueueSubmissionDto(StudentDto Student, SubmissionDto Submission, string Payload);