using Xunit;
using cuzzle_api.Models.Helpers;
using System.Linq;

namespace Tests;

public class TokenTest
{
    [Fact]
    public void ShouldNotMatchTokens() 
    {
        // arrange
        string token1 = TokenHelper.GetRandomToken();
        string token2 = TokenHelper.GetRandomToken();

        // act
        byte[] hash1 = TokenHelper.HashToken(token1);
        byte[] hash2 = TokenHelper.HashToken(token2);

        // assert
        Assert.False(hash1.SequenceEqual(hash2));
    }

    [Fact]
    public void ShouldMatchTokens()
    {
        // arrange
        string token1 = TokenHelper.GetRandomToken();
        string token2 = token1;

        // act
        byte[] hash1 = TokenHelper.HashToken(token1);
        byte[] hash2 = TokenHelper.HashToken(token2);
        
        // assert
        Assert.True(hash1.SequenceEqual(hash2));
    }
}
