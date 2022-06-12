using Xunit;
using cuzzle_api.Models.Helpers;
using System.Linq;

namespace Tests;

public class SecurityTest
{
    private byte[] salt = SecurityHelper.GenerateSalt();

    [Fact]
    public void ShouldGenerateTwoIdenticalHashes()
    {
        // arrange
        string password = "mypass";

        // act
        byte[] hash1 = SecurityHelper.GeneratePasswordHash(password, salt);
        byte[] hash2 = SecurityHelper.GeneratePasswordHash(password, salt);

        // assert
        Assert.True(hash1.SequenceEqual(hash2));
    }

    [Fact]
    public void ShouldGenerateTwoDifferentHashes() 
    {
        // arrange
        string pass1 = "mypass";
        string pass2 = "mypas";

        // act
        byte[] hash1 = SecurityHelper.GeneratePasswordHash(pass1, salt);
        byte[] hash2 = SecurityHelper.GeneratePasswordHash(pass2, salt);

        // assert
        Assert.False(hash1.SequenceEqual(hash2));
    }

    [Fact]
    public void ShouldMatchPasswords() 
    {
        // arrange
        string password = "mypass";

        // act
        byte[] hash = SecurityHelper.GeneratePasswordHash(password, salt);

        // assert
        Assert.True(SecurityHelper.MatchPasswords(password, hash, salt));
    }

    [Fact]
    public void ShouldNotMatchPasswords() 
    {
        // arrange
        string password = "mypass";
        string wrongPass = "wrong";

        // act
        byte[] hash = SecurityHelper.GeneratePasswordHash(password, salt);

        // assert
        Assert.False(SecurityHelper.MatchPasswords(wrongPass, hash, salt));
    }
}
