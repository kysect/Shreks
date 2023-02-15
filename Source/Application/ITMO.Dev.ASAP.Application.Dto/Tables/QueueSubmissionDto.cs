using ITMO.Dev.ASAP.Application.Dto.Study;
using ITMO.Dev.ASAP.Application.Dto.Users;

namespace ITMO.Dev.ASAP.Application.Dto.Tables;

public record QueueSubmissionDto(StudentDto Student, SubmissionDto Submission);