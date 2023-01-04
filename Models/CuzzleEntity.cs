using Npgsql;

namespace cuzzle_api.Models;

public class CuzzleEntity: IDbService
{
    private readonly NpgsqlConnection _connection;

    private const string CONNECTION_STRING = "Host=127.0.0.1;Username=cuzzle_user;Password=;Database=cuzzledb";

    public CuzzleEntity()
    {
        _connection = new NpgsqlConnection(CONNECTION_STRING);
    }

    public T? GetScalar<T>(NpgsqlCommand cmd)
    {
        cmd.Connection = _connection;
        object? result = null;
        try
        {
            cmd.Connection.Open();
            result = cmd.ExecuteScalar();
        }
        catch(Exception ex)
        {
            cmd.Connection.Close();
            Console.WriteLine("We could not execute the query.");
            Console.WriteLine(ex.Message);
            Console.WriteLine(ex.StackTrace);
        }
        cmd.Connection.Close();

        if(result is not null) return (T)result;
        return default(T);
    }

    public T GetObject<T>(NpgsqlCommand cmd) where T : class, new()
    {
        cmd.Connection = _connection;

        NpgsqlDataReader? dr = null;
        try
        {
            cmd.Connection.Open();
            dr = cmd.ExecuteReader();
        }
        catch(Exception ex)
        {
            cmd.Connection.Close();
            Console.WriteLine("We could not get a " + new T().ToString());
            // Log full exception
            // raise exception to capture the filter to show to the user
            Console.WriteLine(ex.Message);
        }
        if(dr is null) return new T();
        if(!dr.HasRows) return new T();

        T obj = Mapper.ConvertToObject<T>(dr);
        cmd.Connection.Close();
        return obj;
    }

    public IEnumerable<T> GetListOfObjests<T>(NpgsqlCommand cmd) where T : class, new()
    {
        cmd.Connection = _connection; // override connection

        NpgsqlDataReader? dr = null;
        try
        {
            cmd.Connection.Open();
            dr = cmd.ExecuteReader();
        }
        catch(Exception ex)
        {
            cmd.Connection.Close();
            Console.WriteLine("We could not get the list of " + new T().ToString());
            Console.WriteLine(ex.Message);
        }
        IEnumerable<T> objects = Mapper.ConvertToObjectList<T>(dr);
        cmd.Connection.Close();
        return objects;
    }

    public bool ExecuteQuery(NpgsqlCommand cmd)
    {
        cmd.Connection = _connection;

        int numRows = 0;
        try
        {
            cmd.Connection.Open();
            numRows = cmd.ExecuteNonQuery();
        }
        catch(Exception ex)
        {
            cmd.Connection.Close();
            Console.WriteLine("We could not execute query");
            Console.WriteLine(ex.Message);
            Console.WriteLine(ex.StackTrace);
            return false;
        }
        cmd.Connection.Close();
        return numRows == 1;
    }
}
