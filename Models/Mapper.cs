using Npgsql;
using FastMember;
using System.Text.RegularExpressions;

public static class Mapper
{
    private const string DB_NAME_REGEX = "(?<=[a-z])([A-Z])(?=[a-z])";
    private const string CLASS_NAME_REGEX = "(?<=[a-z])(_[a-z])(?=[a-z])";

    public static string GetDbName(this string dbName)
    {
        dbName = Regex.Replace(dbName, DB_NAME_REGEX,
                m => string.Format("_{0}", m.Groups[1].Value));

        dbName = dbName.ToLower();
        return dbName;
    }

    public static string GetClassName(this string className)
    {
        className = className.ToLower();
        className = Regex.Replace(className, CLASS_NAME_REGEX, 
                m => string.Format("{0}", Char.ToUpper(m.Groups[1].ValueSpan[1])));
        className = Regex.Replace(className, "_", "");
        className = className[0].ToString().ToUpper() + className.Substring(1);

        return className;
    }

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
            if(rd.IsDBNull(i)) continue;

            string fieldName = rd.GetName(i);
            fieldName = fieldName.GetClassName();
            if (ms.Any(m => string.Equals(m.Name, fieldName,
                            StringComparison.OrdinalIgnoreCase)))
            {
                ta[t, fieldName] = rd.GetValue(i);
            }
        }
        return t;
    }

    public static IEnumerable<T> ConvertToObjectList<T>(this NpgsqlDataReader? rd) where T : class, new()
    {
        if(rd is null) return new List<T>();

        var accessor = TypeAccessor.Create(typeof(T));
        var members = accessor.GetMembers();

        ICollection<T> ts = new List<T>();
        while(rd.Read())
        {
            ts.Add(GetObject<T>(accessor, members, rd));
        }

        return ts;
    }
}
