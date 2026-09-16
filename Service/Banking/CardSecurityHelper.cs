using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Service.Banking
{
    public class CardSecurityHelper
    {
        private readonly string _salt;

        public CardSecurityHelper(IConfiguration configuration)
        {
            // Pulls the salt securely from your User Secrets (in Development) or Environment Variables (in Production)
            _salt = configuration["CardSecurity:SecretKey"]
                ?? throw new InvalidOperationException("Card hash salt is missing from configuration/secrets.");
        }

        public string GenerateRandomCardNumber()
        {
            // Generates a mock 16-digit card number starting with a standard test BIN (e.g., 4242)
            Random random = new Random();
            string prefix = "4242";
            string body = random.Next(100000000, 999999999).ToString() + random.Next(10, 99).ToString();
            return prefix + body; // Total 16 digits
        }

        public string MaskCardNumber(string rawCardNumber)
        {
            if (string.IsNullOrEmpty(rawCardNumber) || rawCardNumber.Length < 4)
                return "****";

            string lastFour = rawCardNumber[^4..];
            return $"****-****-****-{lastFour}";
        }

        public string ComputeHashedValue(string rawInput)
        {
            // Combines the raw input with your secure secret salt to prevent rainbow table attacks
            string saltedInput = rawInput + _salt;

            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(saltedInput);
                byte[] hashBytes = sha256.ComputeHash(bytes);

                // Convert byte array to a 64-character lowercase hex string (perfect for varchar(64))
                StringBuilder sb = new StringBuilder();
                foreach (byte b in hashBytes)
                {
                    sb.Append(b.ToString("x2"));
                }
                return sb.ToString();
            }
        }

        public string GenerateRandomCVV()
        {
            Random random = new Random();
            return random.Next(100, 999).ToString(); // Generates a secure 3-digit CVV
        }
    }
}
