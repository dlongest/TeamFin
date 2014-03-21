using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TeamFin
{
    /// <summary>
    /// Used to indicate that a property is associated with a TFS field of the provided FieldName.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class TfsFieldAttribute : Attribute
    {
        private string _name;

        public TfsFieldAttribute(string name)
        {
            _name = name;
        }

        public string FieldName { get { return _name; } }
    }
}
