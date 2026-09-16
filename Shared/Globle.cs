using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public class Globle
    {
        public async Task<bool> IsFileIntegrityValid(Stream fileStream, string providedHashHex)
        {
            using (var sha256 = SHA256.Create())
            {
                // ComputeHashAsync streams the file; it does NOT load it all at once.
                byte[] computedHashBytes = await sha256.ComputeHashAsync(fileStream);

                // Convert byte array to Hex string to compare with the one sent by the user
                string computedHashHex = Convert.ToHexString(computedHashBytes);

                // Reset stream position to 0 so you can read it again for decryption
                fileStream.Position = 0;

                return string.Equals(computedHashHex, providedHashHex, StringComparison.OrdinalIgnoreCase);
            }
        }
        public async Task ProcessCompressedSalaryFile(Stream networkStream, string expectedHash)
        {
            
            // 1. Setup Decompression Stream
            using var decompressionStream = new GZipStream(networkStream, CompressionMode.Decompress);

            // 2. Wrap it in a HashStream (to calculate hash while reading)
            using var sha256 = SHA256.Create();

            // We use a CryptoStream in 'Read' mode to hash the UNCOMPRESSED data
            using var hashStream = new CryptoStream(decompressionStream, sha256, CryptoStreamMode.Read);

            // 3. Read the data line-by-line
            using var reader = new StreamReader(hashStream);

            while (!reader.EndOfStream)
            {
                string line = await reader.ReadLineAsync();
                // Process the salary record immediately via ADO.NET
              //  await _salaryService.ProcessLine(line);
            }

            // 4. Verify the Hash AFTER the stream is fully read
            byte[] computedHash = sha256.Hash;
            if (Convert.ToHexString(computedHash) != expectedHash)
            {
                throw new SecurityException("Data Integrity Failure: The uncompressed data did not match the hash!");
            }
        }



    }
}
