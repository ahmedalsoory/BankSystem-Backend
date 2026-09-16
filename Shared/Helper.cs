using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public static class Helper
    {
        public static byte[] ParseRowVersion(string version)
        {
            if (string.IsNullOrEmpty(version)) return null;

            if (version.StartsWith("0x"))
            {
                // Hex Conversion Logic
                return Enumerable.Range(0, version.Length - 2)
                                 .Where(x => x % 2 == 0)
                                 .Select(x => Convert.ToByte(version.Substring(x + 2, 2), 16))
                                 .ToArray();
            }

            // Base64 Conversion Logic
            return Convert.FromBase64String(version);
        }
    }
}
