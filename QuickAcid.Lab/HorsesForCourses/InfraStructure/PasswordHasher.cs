using System.Security.Cryptography;

namespace QuickAcid.Lab.HorsesForCourses.InfraStructure;

public interface IPasswordHasher { string Hash(string password); bool Verify(string password, string hash); }

public class Pbkdf2PasswordHasher : IPasswordHasher
{
    private const int SaltSize = 16; // 128-bit
    private const int KeySize = 32;  // 256-bit
    private const int Iterations = 100_000; // good trade-off between speed & security

    public string Hash(string password)
    {
        using var rng = RandomNumberGenerator.Create();
        byte[] salt = new byte[SaltSize];
        rng.GetBytes(salt);

        using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256);
        byte[] key = pbkdf2.GetBytes(KeySize);

        // Store as: {iterations}.{base64Salt}.{base64Key}
        return $"{Iterations}.{Convert.ToBase64String(salt)}.{Convert.ToBase64String(key)}";
    }

    public bool Verify(string password, string hash)
    {
        var parts = hash.Split('.', 3);
        if (parts.Length != 3)
            return false;

        int iterations = Convert.ToInt32(parts[0]);
        byte[] salt = Convert.FromBase64String(parts[1]);
        byte[] key = Convert.FromBase64String(parts[2]);

        using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256);
        byte[] attemptedKey = pbkdf2.GetBytes(KeySize);

        return CryptographicOperations.FixedTimeEquals(attemptedKey, key);
    }
}