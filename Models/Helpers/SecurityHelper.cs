using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace cuzzle_api.Models.Helpers;

public static class SecurityHelper 
{
    private const int SALTBYTES = 64;
    private const int ITERATIONS = 50000;
    private const int NUMBYTES = 256;

    public static byte[] GenerateSalt()
    {
        byte[] salt = new byte[SALTBYTES];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(salt);
        }
        return salt;
    }

    public static byte[] GeneratePasswordHash(string password, byte[] salt)
    {
        byte[] passwordHash = KeyDerivation.Pbkdf2(
            password: password,
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA512,
            iterationCount: ITERATIONS,
            numBytesRequested: NUMBYTES);
        return passwordHash;
    }

    public static bool MatchPasswords(string password, byte[] passwordDb, byte[] saltDb)
    {
        byte[] pass = GeneratePasswordHash(password, saltDb);

        return pass.SequenceEqual(passwordDb);
    }

}
