public interface IPuzzleService 
{
    Puzzle GetPuzzle(Guid id);

    IEnumerable<Puzzle> GetPuzzleList();

    Puzzle AddPuzzle(PuzzleVM puzzleToAdd);

    bool UpdatePuzzle(PuzzleVM puzzleToUpdate);

    bool DeletePuzzle(Guid id);
}
