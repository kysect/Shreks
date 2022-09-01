﻿namespace Kysect.Shreks.Application.Dto.Github;

public record GithubPullRequestDescriptor(
    string Sender,
    string Payload,
    string Organization,
    string Repository,
    string BranchName,
    long PrNumber)
{
    public override string ToString()
    {
        return $"{Payload} with branch {BranchName} from {Sender}";
    }
}