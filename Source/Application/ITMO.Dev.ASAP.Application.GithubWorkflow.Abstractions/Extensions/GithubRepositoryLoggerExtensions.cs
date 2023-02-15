using ITMO.Dev.ASAP.Application.GithubWorkflow.Abstractions.Models;
using ITMO.Dev.ASAP.Common.Logging;
using Microsoft.Extensions.Logging;

namespace ITMO.Dev.ASAP.Application.GithubWorkflow.Abstractions.Extensions;

public static class GithubRepositoryLoggerExtensions
{
    public static ILogger ToRepositoryLogger(this ILogger logger, GithubPullRequestDescriptor descriptor)
    {
        string prefix = $"{descriptor.Organization}/{descriptor.Repository}/{descriptor.PrNumber}";

        return new PrefixLoggerProxy(logger, prefix);
    }
}