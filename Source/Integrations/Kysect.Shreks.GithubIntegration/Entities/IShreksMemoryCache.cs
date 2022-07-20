using Microsoft.Extensions.Caching.Memory;

namespace Kysect.Shreks.GithubIntegration.Entities;

public interface IShreksMemoryCache : IMemoryCache, IDisposable
{
    void Set<T>(string key, T value, TimeSpan expiration);
    T Get<T>(string key);
    void Remove(string key);
}