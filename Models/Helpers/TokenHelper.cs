using System.Security.Cryptography;
using System.Text;

namespace cuzzle_api.Models.Helpers;

public static class TokenHelper 
{
    private const int RANDOMBYTES = 64;
    private const int HASHBYTES = 32;

    public static byte[] HashToken(string token)
    {
        byte[] tokenBytes = Encoding.UTF8.GetBytes(token);
        byte[] hash = new byte[HASHBYTES];
        using(SHA256 sha = SHA256.Create())
        {
            hash = sha.ComputeHash(tokenBytes);
        }
        return hash;
    }

    public static string GetRandomToken()
    {
        string token = string.Empty;
        var randomNumber = new byte[RANDOMBYTES];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomNumber);
            token = Convert.ToBase64String(randomNumber);
        }
        return token;
    }
}
