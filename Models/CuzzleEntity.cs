using Npgsql;

namespace cuzzle_api.Models;

public class CuzzleEntity
{
    private readonly NpgsqlConnection _connection;

    private const string CONNECTION_STRING = "Host=127.0.0.1;Username=cuzzle_user;Password=;Database=cuzzledb";

    public CuzzleEntity()
    {
        _connection = new NpgsqlConnection(CONNECTION_STRING);
    }

    public Puzzle GetData(string query)
    {
        _connection.Open();
        NpgsqlCommand cmd = new NpgsqlCommand(query, _connection);

        NpgsqlDataReader? dr = null;
        try {
            dr = cmd.ExecuteReader();
        }
        catch(Exception ex)
        {
            Console.WriteLine("Here, this is an error!");
            Console.WriteLine(ex.Message);
        }
        Puzzle puzzle = new Puzzle();
        dr.Read();
        puzzle.name = (string)dr["name"];
        _connection.Close();
        return puzzle;
    }
}
