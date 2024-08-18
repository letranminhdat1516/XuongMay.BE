using System.Security.Cryptography;
using System.Text;

namespace XuongMayBE.API.Config
{
    /// <summary>
    /// Helper class to hash passwords using SHA256 double hashing.
    /// </summary>
    public static class HashHelper
    {
        /// <summary>
        /// Computes a SHA256 double hash of the input string.
        /// </summary>
        /// <param name="input">The string to hash.</param>
        /// <returns>The hashed string.</returns>
        public static string ComputeSha256Hash(string input)
        {
            using (var sha256 = SHA256.Create())
            {
                // First hash
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
                var hash = Convert.ToBase64String(hashedBytes);

                // Second hash
                hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(hash));
                return Convert.ToBase64String(hashedBytes);
            }
        }
    }
}
