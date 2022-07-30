using Kysect.Shreks.Integration.Github.Client;
using Microsoft.Extensions.Logging;
using Octokit.Webhooks;

namespace Kysect.Shreks.Integration.Github.Entities;

public class ActionNotifier : IActionNotifier
{
    private readonly ILogger<ActionNotifier> _logger;
    private readonly IInstallationClientFactory _installationClientFactory;

    public ActionNotifier(IInstallationClientFactory installationClientFactory, ILogger<ActionNotifier> logger)
    {
        _installationClientFactory = installationClientFactory;
        _logger = logger;
    }

    public async Task ApplyInComments(WebhookEvent webhookEvent, long issueNumber, string hook)
    {
        var repository = webhookEvent.Repository;
        var installation = webhookEvent.Installation;

        if (repository is null)
        {
            _logger.LogError("Repository to comment on is null");
            throw new ArgumentNullException(nameof(repository), "Repository to comment on must not be null.");
        }

        if (installation is null)
        {
            _logger.LogError($"Installation to comment in repository {repository.Name} is null");
            throw new ArgumentNullException(nameof(installation), "Installation to comment on must not be null.");
        }

        var installationClient = await _installationClientFactory.GetClient(installation.Id);

        await installationClient.Issue.Comment.Create(
            repository.Owner.Login, 
            repository.Name, 
            (int) issueNumber, 
            $"**Hook**: `{hook}` {Environment.NewLine} **Action**: `{webhookEvent.Action}`");
    }
}