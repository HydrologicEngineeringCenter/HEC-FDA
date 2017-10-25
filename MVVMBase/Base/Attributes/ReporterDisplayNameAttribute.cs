using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Base.Attributes
{
    [System.AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class ReporterDisplayNameAttribute: Attribute
    {
        private readonly string _displayName;
        public string DisplayName { get { return _displayName; } }
        public ReporterDisplayNameAttribute(string displayName)
        {
            _displayName = displayName;
        }
    }
}
