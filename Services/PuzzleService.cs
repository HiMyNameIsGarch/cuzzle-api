using cuzzle_api.Models;
using Npgsql;

public class PuzzleService 
{
    private readonly CuzzleEntity _db;

    public PuzzleService()
    {
        _db = new CuzzleEntity();
    }

    public Puzzle GetPuzzle(Guid id)
    {
        NpgsqlCommand cmd = new NpgsqlCommand();
        cmd.CommandText = "SELECT * FROM puzzle WHERE id = @id::UUID;";
        cmd.Parameters.AddWithValue("id", id);

        var puzzle = _db.GetObject<Puzzle>(cmd);
        return puzzle;
    }

    public IEnumerable<Puzzle> GetPuzzleList()
    {
        NpgsqlCommand cmd = new NpgsqlCommand();
        cmd.CommandText = "SELECT * FROM puzzle;";

        var puzzles = _db.GetListOfObjests<Puzzle>(cmd);
        return puzzles;
    }

    public Puzzle AddPuzzle(PuzzleVM puzzleToAdd)
    {
        NpgsqlCommand cmd = new NpgsqlCommand();
        cmd.CommandText = "INSERT INTO puzzle(account_id, name, question, flag) VALUES(@account_id::UUID, @name, @question, @flag) RETURNING *;";
        cmd.Parameters.AddWithValue("account_id", "98b78eae-e872-4097-8415-5ac4fb68fd0b");
        cmd.Parameters.AddWithValue("name", puzzleToAdd.name);
        cmd.Parameters.AddWithValue("question", puzzleToAdd.question);
        cmd.Parameters.AddWithValue("flag", puzzleToAdd.flag);

        var puzzle = _db.GetObject<Puzzle>(cmd);
        return puzzle;
    }

    public bool UpdatePuzzle(PuzzleVM puzzleToUpdate)
    {
        NpgsqlCommand cmd = new NpgsqlCommand();
        cmd.CommandText = "UPDATE puzzle SET name = @name, question = @question, flag = @flag WHERE id = @id::UUID;";
        cmd.Parameters.AddWithValue("id", puzzleToUpdate.id);
        cmd.Parameters.AddWithValue("name", puzzleToUpdate.name);
        cmd.Parameters.AddWithValue("question", puzzleToUpdate.question);
        cmd.Parameters.AddWithValue("flag", puzzleToUpdate.flag);

        var updated = _db.ExecuteQuery(cmd);
        return updated;
    }

    public bool DeletePuzzle(Guid id)
    {
        NpgsqlCommand cmd = new NpgsqlCommand();
        cmd.CommandText = "DELETE FROM puzzle WHERE id = @id::UUID;";
        cmd.Parameters.AddWithValue("id", id);

        var deleted = _db.ExecuteQuery(cmd);
        return deleted;

    }
}
