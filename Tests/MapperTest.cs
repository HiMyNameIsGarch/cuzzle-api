using Xunit;

namespace Tests;

public class MapperTest
{
    [Theory]
    [InlineData("PuzzleId", "puzzle_id")]
    [InlineData("puzzle_Id", "puzzle_id")]
    [InlineData("puzzleid", "puzzleid")]
    public void ShouldFormatDbName(string dbName, string expected)
    {
        // act
        dbName = dbName.GetDbName();
        // assert
        Assert.Equal(dbName, expected);
    }

    [Theory]
    [InlineData("puzzle_id", "PuzzleId")]
    [InlineData("Created_At", "CreatedAt")]
    [InlineData("_allow_anonymous_", "AllowAnonymous")]
    public void ShouldFormatClassName(string className, string expected)
    {
        // act
        className = className.GetClassName();
        // assert
        Assert.Equal(className, expected);
    }
}
