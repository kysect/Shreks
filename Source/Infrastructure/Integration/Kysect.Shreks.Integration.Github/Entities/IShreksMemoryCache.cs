using Microsoft.Extensions.Caching.Memory;

namespace Kysect.Shreks.Integration.Github.Entities;

public interface IShreksMemoryCache : IDisposable
{ 
    TItem GetOrCreate<TItem>(object key, Func<ICacheEntry, TItem> factory);
}