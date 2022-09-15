using Kysect.Shreks.Application.Dto.Github;
using Kysect.Shreks.Common.Logging;
using Microsoft.Extensions.Logging;

namespace Kysect.Shreks.Integration.Github.Helpers;

public static class GithubRepositoryLoggerExtensions
{
    public static ILogger ToRepositoryLogger(this ILogger logger, GithubPullRequestDescriptor descriptor)
    {
        string prefix = $"{descriptor.Organization}/{descriptor.Repository}/{descriptor.PrNumber}";

        return new PrefixLoggerProxy(logger, prefix);
    }
}