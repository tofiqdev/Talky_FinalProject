using System.Security.Cryptography;
using System.Text;

namespace Core.Helpers
{
    public static class PasswordHelper
    {
        public static string HashPassword(string password)
        {
            // Use BCrypt for production - this is a simple implementation
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }

        public static bool VerifyPassword(string password, string hash)
        {
            var hashedPassword = HashPassword(password);
            return hashedPassword == hash;
        }
    }
}
