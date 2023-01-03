using Octokit;

namespace Kysect.Shreks.Integration.Github.CredentialStores;

public class InstallationCredentialStore : ICredentialStore
{
    private readonly IGitHubClient _gitHubAppClient;
    private readonly long _installationId;
    private readonly object _lock = new object();

    private long _expirationUnixTimestamp;
    private volatile Task<Credentials> _task;

    private TokenRefreshState _tokenRefreshState;

    public InstallationCredentialStore(IGitHubClient gitHubAppClient, long installationId)
    {
        _gitHubAppClient = gitHubAppClient;
        _installationId = installationId;
        _tokenRefreshState = new TokenRefreshState();
        _expirationUnixTimestamp = 0;
        _task = Task.FromResult<Credentials>(null!);
    }

    public Task<Credentials> GetCredentials()
    {
        //check if token has expired or is already refreshing in async task
        if (!IsTokenExpired() || _tokenRefreshState.IsRefreshing())
            return _task;
        lock (_lock)
        {
            //check if while waiting for lock token was already set refreshing
            if (!_tokenRefreshState.TrySetStartRefreshing())
                return _task;
            //check if it is not refreshing because it was successfully refreshed while we were waiting for lock
            if (IsTokenExpired())
            {
                _task = RefreshToken();
                _tokenRefreshState.TrySetRefreshing();
            }
            else
            {
                _tokenRefreshState.SetNotRefreshing();
            }
        }

        return _task;
    }

    private async Task<Credentials> RefreshToken()
    {
        try
        {
            AccessToken? token = await _gitHubAppClient.GitHubApps.CreateInstallationToken(_installationId);
            SetExpirationDate(token.ExpiresAt.AddMinutes(-1));
            return new Credentials(token.Token);
        }
        catch
        {
            SetExpirationDate(DateTimeOffset.Now);
            throw;
        }
        finally
        {
            _tokenRefreshState.SetNotRefreshing();
        }
    }

    private void SetExpirationDate(DateTimeOffset dateTimeOffset)
    {
        Volatile.Write(ref _expirationUnixTimestamp, dateTimeOffset.ToUnixTimeSeconds());
    }

    private bool IsTokenExpired()
    {
        return Volatile.Read(ref _expirationUnixTimestamp) <= DateTimeOffset.Now.ToUnixTimeSeconds();
    }

    private struct TokenRefreshState
    {
        private enum State
        {
            NotRefreshing, //refreshing task is not running
            StartRefreshing, //refreshing task is being created
            Refreshing, //refreshing task is running
        }

        private volatile int _state;

        public TokenRefreshState()
        {
            _state = (int)State.NotRefreshing;
        }

        public bool IsRefreshing()
        {
            return _state == (int)State.Refreshing;
        }

        public void SetNotRefreshing()
        {
            _state = (int)State.NotRefreshing;
        }

        //if not refreshing, advance to starting refreshing (creating refreshing task)
        public bool TrySetStartRefreshing()
        {
            return Interlocked.CompareExchange(ref _state, (int)State.StartRefreshing,
                (int)State.NotRefreshing) == (int)State.NotRefreshing;
        }

        //if started refreshing, advance to actually refreshing (task is running)
        public bool TrySetRefreshing()
        {
            return Interlocked.CompareExchange(ref _state, (int)State.Refreshing,
                (int)State.StartRefreshing) == (int)State.StartRefreshing;
        }
    }
}