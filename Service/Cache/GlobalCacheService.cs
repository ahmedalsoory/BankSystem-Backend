using System;
using System.Collections.Concurrent;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ServiceContract;
/*
namespace Service.Cache
{
    public class GlobalCacheService : IEntityCache
    {
        // Change from Type to CacheType (Enum)
        private readonly ConcurrentDictionary<CacheType, EntityBitSet> _stores = new();
        private volatile bool _isReady = false;

        // Helper to get store by Enum instead of Type
        private EntityBitSet GetStore(CacheType type) =>
            _stores.GetOrAdd(type, _ => new EntityBitSet(1024));

        // Refactored to use Enum
        public bool IsActive(CacheType type, int id) =>
            _stores.TryGetValue(type, out var bitSet) && bitSet.IsActive(id);

        public void SetStatus(CacheType type, int id, bool active) =>
            GetStore(type).SetStatus(id, active);

        public int GetLastId(CacheType type) =>
            GetStore(type).LastId;

        public void UpdateLastId(CacheType type, int id) =>
            GetStore(type).UpdateLastId(id);

        public async Task Initialize(CacheType type, int lastId, IAsyncEnumerable<int> activeIds)
        {
            // Initializing with exact size to prevent 'Grow' allocations immediately
            var newStore = new EntityBitSet(lastId);
            newStore.UpdateLastId(lastId);

            await foreach (var id in activeIds)
            {
                newStore.SetStatus(id, true);
            }

            // ATOMIC SWAP
            _stores[type] = newStore;
        }

        public void MarkAsReady() => _isReady = true;
        public bool IsShieldReady() => _isReady;

        private class EntityBitSet
        {
            private int _lastId;
            private BitArray _bits;
            private readonly object _lock = new();

            public int LastId => _lastId;

            public EntityBitSet(int initialSize)
            {
                // Ensure size is at least 64 and aligned
                int size = Math.Max(64, ((initialSize / 64) + 1) * 64);
                _bits = new BitArray(size, false);
            }

            public bool IsActive(int id)
            {
                var currentBits = _bits; // Thread-safe snapshot
                if (id < 0 || id >= currentBits.Length) return false;
                return currentBits.Get(id);
            }

            public void SetStatus(int id, bool active)
            {
                lock (_lock)
                {
                    // If ID is larger than current array, expand it
                    if (id >= _bits.Length) Grow(id);
                    _bits.Set(id, active);
                    if (id > _lastId) _lastId = id;
                }
            }

            public void UpdateLastId(int id)
            {
                int initial, computed;
                do
                {
                    initial = _lastId;
                    computed = Math.Max(initial, id);
                } while (Interlocked.CompareExchange(ref _lastId, computed, initial) != initial);
            }

            private void Grow(int requiredId)
            {
                // Growth strategy: 1.5x or required, whichever is larger, aligned to 64
                int newSize = (((Math.Max(_bits.Length * 3 / 2, requiredId + 1)) / 64) + 1) * 64;
                var newBits = new BitArray(newSize, false);

                // .Or is efficient for copying BitArrays
                newBits.Or(_bits);
                _bits = newBits;
            }
        }
    }
}*/