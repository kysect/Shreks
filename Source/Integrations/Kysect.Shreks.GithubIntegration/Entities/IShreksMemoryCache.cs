using Microsoft.Extensions.Caching.Memory;

namespace Kysect.Shreks.GithubIntegration.Entities;

public interface IShreksMemoryCache : IMemoryCache
{ 
    Task<TItem> GetOrCreateAsync<TItem>(object key, Func<ICacheEntry, Task<TItem>> factory);
}