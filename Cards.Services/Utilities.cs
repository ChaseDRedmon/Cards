using BCrypt.Net;

namespace Cards.Services
{
    public static class Utilities
    {
        /// <summary>
        /// Hash password 
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public static string HashPassword(string password)
        {
            var hashedPassword = BCrypt.Net.BCrypt.EnhancedHashPassword(password, 12, HashType.SHA512);
            return hashedPassword;
        }
    }
}