using Octokit;

namespace Kysect.Shreks.GithubIntegration.CredentialStores;

public class InstallationCredentialStore : ICredentialStore
{
    private enum TokenRefreshState
    {
        NotRefreshing,
        StartRefreshing,
        Refreshing
    }

    private readonly IGitHubClient _gitHubAppClient;
    private readonly long _installationId;

    private long _expirationUnixTimestamp;
    private volatile Task<Credentials> _task;

    private volatile int _tokenRefreshState;

    public InstallationCredentialStore(IGitHubClient gitHubAppClient, long installationId)
    {
        _gitHubAppClient = gitHubAppClient;
        _installationId = installationId;
        _tokenRefreshState = (int)TokenRefreshState.NotRefreshing;
        _expirationUnixTimestamp = 0;
        _task = Task.FromResult<Credentials>(null!);
    }

    private async Task<Credentials> GetTask()
    {
        try
        {
            var token = await _gitHubAppClient.GitHubApps.CreateInstallationToken(_installationId);
            Volatile.Write(ref _expirationUnixTimestamp, token.ExpiresAt.AddMinutes(-1).ToUnixTimeSeconds());
            return new Credentials(token.Token);
        }
        catch
        {
            Volatile.Write(ref _expirationUnixTimestamp, DateTimeOffset.Now.ToUnixTimeSeconds());
            throw;
        }
        finally
        {
            _tokenRefreshState = (int)TokenRefreshState.NotRefreshing;
        }
    }

    public Task<Credentials> GetCredentials()
    {
        if (Volatile.Read(ref _expirationUnixTimestamp) <= DateTimeOffset.Now.ToUnixTimeSeconds() &&
            _tokenRefreshState != (int)TokenRefreshState.Refreshing)
        {
            lock (this)
            {
                if (Interlocked.CompareExchange(ref _tokenRefreshState, (int)TokenRefreshState.StartRefreshing,
                        (int)TokenRefreshState.NotRefreshing) == (int)TokenRefreshState.NotRefreshing)
                {
                    if (Volatile.Read(ref _expirationUnixTimestamp) <= DateTimeOffset.Now.ToUnixTimeSeconds())
                    {
                        _task = GetTask();
                        Interlocked.CompareExchange(ref _tokenRefreshState, (int)TokenRefreshState.Refreshing,
                            (int)TokenRefreshState.StartRefreshing);
                    }
                    else
                    {
                        _tokenRefreshState = (int)TokenRefreshState.NotRefreshing;
                    }
                }
            }
        }

        return _task;
    }
}