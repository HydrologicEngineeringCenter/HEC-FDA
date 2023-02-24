using System;

namespace HEC.MVVMFramework.Base.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class ReporterDisplayNameAttribute : Attribute
    {
        private readonly string _displayName;
        public string DisplayName { get { return _displayName; } }
        public ReporterDisplayNameAttribute(string displayName)
        {
            _displayName = displayName;
        }
    }
}
