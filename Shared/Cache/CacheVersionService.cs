using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Cache
{
    public class CacheVersionService : ICacheVersionService
    {
        private readonly IMemoryCache _cache;

        // Tracks active cache keys safely across multiple threads without blocking reads
        private readonly ConcurrentDictionary<string, byte> _activeKeys = new(StringComparer.OrdinalIgnoreCase);

        public CacheVersionService(IMemoryCache cache) => _cache = cache;

        public string GetVersion(string key)
        {
            // Safely register the key to our active tracker
            _activeKeys.TryAdd(key, 0);

            // GetOrCreate is internally thread-safe in IMemoryCache
            return _cache.GetOrCreate(key, _ => Guid.NewGuid().ToString())!;
        }

        public void Invalidate(params string[] keys)
        {
            foreach (var key in keys)
            {
                if (!string.IsNullOrWhiteSpace(key))
                {
                    _cache.Set(key, Guid.NewGuid().ToString());
                    _activeKeys.TryAdd(key, 0);
                }
            }
        }

        public void InvalidatePrefix(string prefix)
        {
            // Find all active keys where the first token (split by underscore) matches the prefix exactly
            var keysToInvalidate = _activeKeys.Keys
                .Where(k =>
                {
                    var parts = k.Split('_', StringSplitOptions.RemoveEmptyEntries);
                    return parts.Length > 0 && parts[0].Equals(prefix, StringComparison.OrdinalIgnoreCase);
                })
                .ToArray();

            foreach (var key in keysToInvalidate)
            {
                _cache.Set(key, Guid.NewGuid().ToString());
            }
        }
    }
}
