using System.Data;
using Dapper;

namespace FinApp.Data.CustomMapping
{
    // EnumTypeHandler.cs
    public class EnumTypeHandler<T> : SqlMapper.TypeHandler<T>
        where T : Enum
    {
        public override T Parse(object value)
        {
            if (value == null || DBNull.Value.Equals(value))
                return default; // Return the default value for the enum, e.g., `YourEnumType.SomeValue`

            return (T)Enum.Parse(typeof(T), value.ToString().ToUpper(), true);
        }

        public override void SetValue(IDbDataParameter parameter, T value)
        {
            parameter.Value = value.ToString().ToUpper();
        }
    }
}
