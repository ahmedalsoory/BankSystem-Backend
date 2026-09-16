using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/*
namespace ServiceContract
{
    public enum CacheType : byte // Using 'byte' saves even more RAM than the default 'int'
    {
        User = 1,
        Account = 2,
        Transaction = 3,
        GlobalSetting = 4,
        Client = 5
    }

    public interface IEntityCache
    {
        // No more <T> or 'Type' objects!
        bool IsActive(CacheType type, int id);
        void SetStatus(CacheType type, int id, bool active);

        int GetLastId(CacheType type);
        void UpdateLastId(CacheType type, int id);

        // We use IAsyncEnumerable to keep the "Stream" of IDs memory-efficient
        Task Initialize(CacheType type, int lastId, IAsyncEnumerable<int> activeIds);

        void MarkAsReady();
    }
}
*/