using Microsoft.Extensions.Caching.Memory;

namespace Kysect.Shreks.GithubIntegration.Entities;

public class ShreksMemoryCache : IShreksMemoryCache
{
    private readonly MemoryCache _cache;

    public ShreksMemoryCache(MemoryCacheOptions memoryCacheOptions)
    {
        _cache = new MemoryCache(memoryCacheOptions);
    }

    public ICacheEntry CreateEntry(object key) => _cache.CreateEntry(key);

    public void Remove(object key) => _cache.Remove(key);

    public bool TryGetValue(object key, out object value) => _cache.TryGetValue(key, out value);

    public void Set<T>(string key, T value, TimeSpan expiration) => _cache.Set(key, value, expiration);

    public T Get<T>(string key) => _cache.Get<T>(key);

    public void Remove(string key) => _cache.Remove(key);

    public void Dispose() => _cache.Dispose();
}