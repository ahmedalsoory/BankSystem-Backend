using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Cache
{
    public interface ICacheVersionService
    {
        string GetVersion(string key);
        void Invalidate(params string[] keys);
        void InvalidatePrefix(string prefix);
    }
}
