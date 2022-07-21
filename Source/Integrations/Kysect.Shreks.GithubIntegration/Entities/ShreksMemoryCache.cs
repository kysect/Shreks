using Microsoft.Extensions.Caching.Memory;

namespace Kysect.Shreks.GithubIntegration.Entities;

public class ShreksMemoryCache : IShreksMemoryCache
{
    private readonly MemoryCache _cache;
    private readonly MemoryCacheEntryOptions _cacheEntryOptions;

    public ShreksMemoryCache(
        MemoryCacheOptions memoryCacheOptions, 
        MemoryCacheEntryOptions memoryCacheEntryOptions)
    {
        _cache = new MemoryCache(memoryCacheOptions);
        _cacheEntryOptions = memoryCacheEntryOptions;
    }

    public bool TryGetValue(object key, out object value)
        => _cache.TryGetValue(key, out value);

    public async Task<TItem> GetOrCreateAsync<TItem>(object key,
        Func<ICacheEntry, Task<TItem>> factory)
    {
        return await _cache.GetOrCreateAsync(key, async entry =>
        {
            entry.SetOptions(_cacheEntryOptions);
            return await factory(entry);
        });
    }

    public ICacheEntry CreateEntry(object key) => _cache.CreateEntry(key);
    public void Remove(object key) => _cache.Remove(key);
    public void Dispose() => _cache.Dispose();
}