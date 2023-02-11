using Microsoft.Extensions.Caching.Memory;

namespace ITMO.Dev.ASAP.Integration.Github.Entities;

public interface IAsapMemoryCache : IDisposable
{
    TItem GetOrCreate<TItem>(object key, Func<ICacheEntry, TItem> factory);
}