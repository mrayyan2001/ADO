using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace api.Helpers
{
    public static class HashHelper
    {
        public static string ComputeSha512(string input)
        {
            using (var sha512 = SHA512.Create())
            {
                // 1. Convert input into bytes
                var inputBytes = Encoding.UTF8.GetBytes(input);
                // 2. Compute the hash
                var hashBytes = sha512.ComputeHash(inputBytes);
                // 3. Convert the hash bytes into hex string
                StringBuilder sb = new StringBuilder();
                foreach (var b in hashBytes)
                {
                    sb.Append(b.ToString("x2")); // x2 = lowercase hex
                }
                return sb.ToString();
            }
        }
    }
}