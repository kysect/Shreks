﻿using Kysect.Shreks.Application.Dto.Github;
using Kysect.Shreks.Application.Dto.Study;

namespace Kysect.Shreks.Application.Commands.Submissions;

public interface ICommandSubmissionFactory
{
    public Task<SubmissionRateDto> CreateSubmission(Guid userId, Guid assignmentId, GithubPullRequestDescriptor pullRequestDescriptor);
}