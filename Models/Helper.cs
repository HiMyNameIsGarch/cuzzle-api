using Npgsql;
using FastMember;

public static class Helper
{
    public static T ConvertToObject<T>(this NpgsqlDataReader? rd) where T : class, new()
    {
        if(rd is null) return new T();
        bool moreRows = rd.Read();
        Type type = typeof(T);
        var accessor = TypeAccessor.Create(type);
        var members = accessor.GetMembers();

        var t = GetObject<T>(accessor, members, rd);

        return t;
    }

    private static T GetObject<T>(TypeAccessor ta, MemberSet ms, NpgsqlDataReader rd) where T: class, new()
    {
        var t = new T();

        for (int i = 0; i < rd.FieldCount; i++)
        {
            if (!rd.IsDBNull(i))
            {
                string fieldName = rd.GetName(i);

                if (ms.Any(m => string.Equals(m.Name, fieldName, StringComparison.OrdinalIgnoreCase)))
                {
                    ta[t, fieldName] = rd.GetValue(i);
                }
            }
        }
        return t;
    }

    public static IEnumerable<T> ConvertToObjectList<T>(this NpgsqlDataReader? rd) where T : class, new()
    {
        if(rd is null) return new List<T>();
        ICollection<T> ts = new List<T>();

        Type type = typeof(T);
        var accessor = TypeAccessor.Create(type);
        var members = accessor.GetMembers();

        while(rd.Read())
        {
            ts.Add(GetObject<T>(accessor, members, rd));
        }

        return ts;
    }
}
