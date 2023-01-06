using Kysect.Shreks.Application.GithubWorkflow.Abstractions.Models;
using Kysect.Shreks.Common.Logging;
using Microsoft.Extensions.Logging;

namespace Kysect.Shreks.Application.GithubWorkflow.Abstractions.Extensions;

public static class GithubRepositoryLoggerExtensions
{
    public static ILogger ToRepositoryLogger(this ILogger logger, GithubPullRequestDescriptor descriptor)
    {
        string prefix = $"{descriptor.Organization}/{descriptor.Repository}/{descriptor.PrNumber}";

        return new PrefixLoggerProxy(logger, prefix);
    }
}