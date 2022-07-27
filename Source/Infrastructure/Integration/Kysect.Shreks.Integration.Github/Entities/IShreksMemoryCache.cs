using Microsoft.Extensions.Caching.Memory;

namespace Kysect.Shreks.Integration.Github.Entities;

public interface IShreksMemoryCache : IMemoryCache
{ 
    Task<TItem> GetOrCreateAsync<TItem>(object key, Func<ICacheEntry, Task<TItem>> factory);
}