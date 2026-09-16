using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

    /*
namespace Service.Cache
{
    public  static class CacheHelper
    {
        public static async ValueTask<T?> GetOrSetAsync<T>(
            IMemoryCache cache,
            string key,
            Func<Task<T?>> databaseQuery) where T : class
        {
            // 1. Try to get from RAM
            if (cache.TryGetValue(key, out T? result))
            {
                return result;
            }

            // 2. If not found, execute the actual DB logic
            result = await databaseQuery();

            // 3. Store in RAM if not null
            if (result != null)
            {
                var options = new MemoryCacheEntryOptions()
                    .SetSize(1)
                    .SetSlidingExpiration(TimeSpan.FromMinutes(5));
                cache.Set(key, result, options);
            }

            return result;
        }

        // A specialized helper for IAsyncEnumerable
        public static async IAsyncEnumerable<T> GetOrSetStreamAsync<T>(
     IMemoryCache cache,
     string key,
     Func<IAsyncEnumerable<T>> factory)
        {
            // 1. Try to get from Cache
            if (cache.TryGetValue(key, out List<T> cachedList))
            {
                if (cachedList != null)
                {
                    foreach (var item in cachedList) yield return item;
                    yield break;
                }
            }

            // 2. Buffer while streaming
            var buffer = new List<T>();
            await foreach (var item in factory())
            {
                buffer.Add(item);
                yield return item;
            }

            // 3. THE CRITICAL FIX for line 57
            if (buffer.Count > 0)
            {
                // Use MemoryCacheEntryOptions to satisfy the "SizeLimit" requirement
                var options = new MemoryCacheEntryOptions()
                    .SetSize(1) // Every page counts as 1 unit of your 1024 limit
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(5));

                cache.Set(key, buffer, options);
            }
        }
    }
}
  */