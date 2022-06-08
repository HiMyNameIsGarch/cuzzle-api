using Xunit;
using System.Linq;
using System;
using cuzzle_api.Models;
using cuzzle_api.Services.PuzzleService;

namespace Tests;

public class PuzzleTest
{
    private readonly PuzzleVM puzzle = new PuzzleVM("Hard", "my puzzle", "a description");

    private readonly PuzzleService ps = new PuzzleService(new CuzzleEntity());

    [Fact]
    public void ShouldAddPuzzleAndDeleteIt()
    {
        // act
        Puzzle created = ps.AddPuzzle(puzzle);

        // assert
        Assert.True(created.Id != Guid.Empty);
        
        // clean up
        ps.DeletePuzzle(created.Id);
    }

    [Fact]
    public void ShouldGetAllPuzzles()
    {
        // arrange
        var created = ps.AddPuzzle(puzzle);

        // act
        var puzzles = ps.GetPuzzleList();

        // assert
        Assert.True(puzzles.Count() > 0);

        //clean up
        ps.DeletePuzzle(created.Id);
    }

    [Theory]
    [InlineData("ddf1cb54-53d2-45bb-b01a-f2eeae7cb249")] // random one
    [InlineData("00000000-0000-0000-0000-000000000000")] // empty one
    public void ShouldNotDeletePuzzle(string id)
    {
        // arrange
        var guid = Guid.Parse(id);
        
        // act
        var deleted = ps.DeletePuzzle(guid);

        // assert
        Assert.False(deleted);
    }

    [Fact]
    public void ShouldAddUpdateAndDeleteAPuzzle()
    {
        // arrange
        Puzzle created = ps.AddPuzzle(puzzle);
        puzzle.Id = created.Id;
        puzzle.Name = "Updated puzzle";

        // act
        bool updated = ps.UpdatePuzzle(puzzle);

        // assert
        Assert.True(updated);

        // clean up
        ps.DeletePuzzle(puzzle.Id);
    }

    [Theory]
    [InlineData("ddf1cb54-53d2-45bb-b01a-f2eeae7cb249")] // random one
    [InlineData("00000000-0000-0000-0000-000000000000")] // empty one
    public void ShouldNotUpdatePuzzle(string id)
    {
        // arrange
        var guid = Guid.Parse(id);
        puzzle.Id = guid;
        puzzle.Name = "updated puzzle";

        // act
        var updated = ps.UpdatePuzzle(puzzle);
        
        // assert
        Assert.False(updated);
    }
}
