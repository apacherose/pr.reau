using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace PropertyRegister.REAU.Persistence
{
    public class DataContextHelper
    {
        public static CustomPropertyTypeMap ColumnMap<T>(Dictionary<string, string> columnMappings = null)
        {
            return new CustomPropertyTypeMap(typeof(T),
                                    (type, columnName) =>
                                    {
                                        return type.GetProperties().FirstOrDefault(prop => string.Compare(GetMappedColumnName(prop, columnMappings), columnName, true) == 0);
                                    });
        }

        private static string GetMappedColumnName(MemberInfo member, Dictionary<string, string> columnMappings)
        {
            if (member == null)
                return null;

            var attrib = (DapperColumnAttribute)Attribute.GetCustomAttribute(member, typeof(DapperColumnAttribute), false);

            if (attrib != null)
                return attrib.Name;

            string mappedPropName = null;
            if (columnMappings?.Any() == true
                && columnMappings.TryGetValue(member.Name, out mappedPropName))
                return mappedPropName;

            return null;
        }
    }
}
