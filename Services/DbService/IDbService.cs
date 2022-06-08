using Npgsql;

public interface IDbService
{
    T? GetScalar<T>(NpgsqlCommand cmd);

    T GetObject<T>(NpgsqlCommand cmd) where T : class, new();

    bool ExecuteQuery(NpgsqlCommand cmd);

    IEnumerable<T> GetListOfObjests<T>(NpgsqlCommand cmd) where T : class, new();
}

