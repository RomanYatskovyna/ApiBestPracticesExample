namespace ApiBestPracticesExample.Infrastructure.Services;

public static class PasswordEncrypter
{
    private const int WorkFactor = 12; // Adjust according to your needs

// Generate a salted and hashed password
    public static string HashPassword(string password)
    {
        var salt = BCrypt.Net.BCrypt.GenerateSalt(WorkFactor);
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password, salt);
        return hashedPassword;
    }

// Verify a password against a stored hash
    public static bool VerifyPassword(string password, string hashedPassword)
    {
        return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
    }
}