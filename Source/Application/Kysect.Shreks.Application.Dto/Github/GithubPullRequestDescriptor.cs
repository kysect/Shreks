namespace Kysect.Shreks.Application.Dto.Github;

public record GithubPullRequestDescriptor(
    string Payload,
    string Organization,
    string Repository,
    string BranchName,
    long PrNumber);