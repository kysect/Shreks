using Microsoft.Extensions.Caching.Memory;

namespace ITMO.Dev.ASAP.Integration.Github.Entities;

public class AsapMemoryCache : IAsapMemoryCache
{
    private readonly IMemoryCache _cache;
    private readonly MemoryCacheEntryOptions _cacheEntryOptions;

    public AsapMemoryCache(
        MemoryCacheOptions memoryCacheOptions,
        MemoryCacheEntryOptions memoryCacheEntryOptions)
    {
        _cache = new MemoryCache(memoryCacheOptions);
        _cacheEntryOptions = memoryCacheEntryOptions;
    }

    public TItem GetOrCreate<TItem>(object key, Func<ICacheEntry, TItem> factory)
    {
        return _cache.GetOrCreate(key, entry =>
        {
            entry.SetOptions(_cacheEntryOptions);
            return factory(entry);
        });
    }

    public void Dispose()
    {
        _cache.Dispose();
    }
}