using System.Security.Cryptography;
using System.Text;

namespace StudentPortal.Web.Helpers
{
    public static class PasswordHelper
    {
        // Hash the password with MD5 (same format as your DB: base64 string)
        public static string HashPassword(string password)
        {
            using var md5 = MD5.Create();
            var inputBytes = Encoding.UTF8.GetBytes(password);
            var hashBytes = md5.ComputeHash(inputBytes);
            return Convert.ToBase64String(hashBytes);
        }

        // Verify a raw password against the stored hash
        public static bool VerifyPassword(string storedHash, string password)
        {
            var hashed = HashPassword(password);
            return storedHash == hashed;
        }
    }
}
