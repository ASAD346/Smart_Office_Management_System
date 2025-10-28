namespace SOMS.API.Helpers
{
    public static class PasswordHasher
    {
        public static string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public static bool VerifyPassword(string password, string hash)
        {
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }

        // For testing - generates hash for "admin123"
        public static void GenerateTestHash()
        {
            string password = "admin123";
            string hash = HashPassword(password);
            Console.WriteLine($"Password: {password}");
            Console.WriteLine($"Hash: {hash}");
        }
    }
}
