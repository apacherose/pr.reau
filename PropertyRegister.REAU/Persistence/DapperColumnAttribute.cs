using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyRegister.REAU.Persistence
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class DapperColumnAttribute : Attribute
    {
        private string _name;

        public DapperColumnAttribute(string name)
        {
            this._name = name;
        }

        public string Name { get { return _name; } }
    }
}
