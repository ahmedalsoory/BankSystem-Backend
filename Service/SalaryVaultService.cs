using System.Security.Cryptography;
using System.Net;
using System.Net.Sockets;
using System.Security;
using Serilog;

namespace Service
{


    public class SalaryVaultService
    {
        private readonly string _allowedGovIp = "10.0.0.5"; // Government's Fixed IP
        private readonly ILogger _logger;

        public SalaryVaultService(ILogger logger) => _logger = logger;

        public async Task ProcessSecureUpload(TcpClient client, Stream salaryStream, byte[] providedSignature, long companyId)
        {
            // 1. IP GUARD: Immediate rejection of unknown sources
            var remoteIp = ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString();
            if (remoteIp != _allowedGovIp)
            {
                await AlertSecurityTeam($"UNAUTHORIZED IP ATTEMPT: {remoteIp}");
                client.Close();
                return;
            }

            // 2. STREAMING HASH: Calculate hash while reading the file (RAM SAFE)
            using var sha256 = SHA256.Create();
            byte[] fileHash = await sha256.ComputeHashAsync(salaryStream);

            // 3. SIGNATURE VERIFY: The "Bypass" you mentioned
            bool isAuthentic = await VerifyGovernmentSignature(fileHash, providedSignature, companyId);

            if (isAuthentic)
            {
                _logger.Information("Signature Valid. Proceeding to Internal Clearing Lane.");
                // Proceed to your ADO.NET Bulk Insert logic here...
            }
            else
            {
                await AlertSecurityTeam($"INVALID SIGNATURE detected for Company {companyId}. File rejected.");
                throw new SecurityException("Data Integrity Failure: Signature does not match file hash.");
            }
        }

        private async Task<bool> VerifyGovernmentSignature(byte[] hash, byte[] signature, long id)
        {
            // Fetch Public Key from your DB Table (Logic we discussed earlier)
            //    byte[] publicKey = await GetPublicKeyFromDatabase(id);
            byte[] publicKey = new byte[21];
            using var rsa = RSA.Create();
            rsa.ImportRSAPublicKey(publicKey, out _);

            return rsa.VerifyHash(hash, signature, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        }

        private async Task AlertSecurityTeam(string message)
        {
            // Fire-and-forget alert (Suggestion 3)
            // Log to a separate secure table or send a UDP packet to the monitor
            _logger.Warning($"[SECURITY ALERT] {message} at {DateTime.UtcNow}");
        }
    }
}