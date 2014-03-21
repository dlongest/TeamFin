using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TeamFin.Tests
{
    /// <summary>
    /// For testing purposes, simply maps all property names in type T to themselves (i.e. a property of name "A" is both the key
    /// and value).
    /// </summary>
    public static class FieldMappings
    {
        public static IDictionary<string, string> Map<T>()
        {
            var type = typeof(T);

            var propNames = type.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance).Select(a => a.Name);

            return propNames.ToDictionary(a => a, a => a);
        }
    }
}
