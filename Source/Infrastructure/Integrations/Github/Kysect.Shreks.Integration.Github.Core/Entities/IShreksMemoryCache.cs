using Microsoft.Extensions.Caching.Memory;

namespace Kysect.Shreks.Integration.Github.Core.Entities;

public interface IShreksMemoryCache : IDisposable
{ 
    Task<TItem> GetOrCreateAsync<TItem>(object key, Func<ICacheEntry, Task<TItem>> factory);
}