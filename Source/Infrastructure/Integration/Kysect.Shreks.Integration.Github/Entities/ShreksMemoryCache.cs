using Microsoft.Extensions.Caching.Memory;

namespace Kysect.Shreks.Integration.Github.Entities;

public class ShreksMemoryCache : IShreksMemoryCache
{
    private readonly IMemoryCache _cache;
    private readonly MemoryCacheEntryOptions _cacheEntryOptions;

    public ShreksMemoryCache(
        MemoryCacheOptions memoryCacheOptions, 
        MemoryCacheEntryOptions memoryCacheEntryOptions)
    {
        _cache = new MemoryCache(memoryCacheOptions);
        _cacheEntryOptions = memoryCacheEntryOptions;
    }

    public async Task<TItem> GetOrCreateAsync<TItem>(object key,
        Func<ICacheEntry, Task<TItem>> factory)
    {
        return await _cache.GetOrCreateAsync(key, async entry =>
        {
            entry.SetOptions(_cacheEntryOptions);
            return await factory(entry);
        });
    }

    public void Dispose() => _cache.Dispose();
}